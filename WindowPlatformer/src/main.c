#include "SDL3/SDL.h"
#include "SDL3/SDL_main.h"
#include "application.h"
#include "level.h"
#include "stdlib.h"
#include "stdio.h"
#include "renderer.h"
#include "game_state.h"
#include "math.h"
#include "input.h"
#include "utils.h"
#include "unistd.h"

static void handle_collision(V2f *newPos, V2f *vel, bool *grounded, Object **col) {
    Object pl = *gameState.player;

    f32 nl = newPos->x - pl.size.x / 2.0f;
    f32 nr = newPos->x + pl.size.x / 2.0f;
    f32 nt = newPos->y + pl.size.y / 2.0f;
    f32 nb = newPos->y - pl.size.y / 2.0f;

    *grounded = false;
    *col = NULL;

    for(size_t i = 0; i < gameState.objCount; i++) {
        Object *obj = gameState.objects[i];

        if(!obj->enabled) {
            continue;
        }

        f32 wl = obj->loc.x - obj->size.x / 2.0f;
        f32 wr = obj->loc.x + obj->size.x / 2.0f;
        f32 wt = obj->loc.y + obj->size.y / 2.0f;
        f32 wb = obj->loc.y - obj->size.y / 2.0f;

        if(nr > wl && nl < wr) {
            if(pl.loc.y + pl.size.y/2.0f > wb && pl.loc.y - pl.size.y/2.0f < wt) {
                // TODO: portals
                if(newPos->x > pl.loc.x) {
                    newPos->x = wl - pl.size.x/2.0f;
                }
                else if(newPos->x < pl.loc.x) {
                    newPos->x = wr + pl.size.x/2.0f;
                }

                vel->x = 0.0f;
                *col = obj;
            }
        }

        if(nb < wt && nt > wb) {
            if(pl.loc.x + pl.size.x/2.0f > wl && pl.loc.x - pl.size.x/2.0f < wr) {
                if(newPos->y > pl.loc.y) {
                    newPos->y = wb - pl.size.y/2.0f;
                }
                else if(newPos->y < pl.loc.y) {
                    newPos->y = wt + pl.size.y/2.0f;
                    *grounded = true;
                }

                vel->y = 0.0f;
                *col = obj;
            }
        }
    } 
}

static bool rects_intersect(V2f l1, V2f s1, V2f l2, V2f s2) {
    return
        maxf(l1.x, l2.x) < minf(l1.x + s1.x, l2.x + s2.x) &&
        maxf(l1.y, l2.y) < minf(l1.y + s1.y, l2.y + s2.y);
}

static void tick(float dt) {
    static V2f vel = {0, 0};
    static bool grounded = false;

    if(!gameState.loadedLevel) {
        vel = (V2f){0, 0};
        return;
    }

    const f32 GRAVITY = -10.0f;
    const f32 STOMP_SPEED = -40.0f;
    const f32 JUMP_STRENGTH = 3.25f;
    const f32 H_ACC = 40.0f;
    const f32 H_DAMP = 60.0f;
    const f32 MAX_H_SPEED = 2.25f;

    vel.y += GRAVITY * dt;

    bool rInput = key_helt(KC_RIGHT) || key_helt(KC_D);
    bool lInput = key_helt(KC_RIGHT) || key_helt(KC_A);

    if(rInput) {
        vel.x += H_ACC * dt;
    }
    if(lInput) {
        vel.x -= H_ACC * dt;
    }
    if(!rInput && !lInput) {
        vel.x -= signf(vel.x) * H_DAMP * dt;

        if(abs(vel.x) < 0.3f) {
            vel.x = 0.0f;
        }
    }

    vel.x = clampf(vel.x, -MAX_H_SPEED, MAX_H_SPEED);

    if(key_helt(KC_DOWN) || key_helt(KC_S)) {
        vel.y += STOMP_SPEED * dt;
    }

    if(grounded && (key_down(KC_W) || key_down(KC_UP) || key_down(KC_SPACE))) {
        vel.y = JUMP_STRENGTH;
    }

    V2f newPos = {
        gameState.player->loc.x + vel.x * dt,
        gameState.player->loc.y + vel.y * dt
    };

    Object *col;
    handle_collision(&newPos, &vel, &grounded, &col);

    gameState.player->loc = newPos;
    obj_on_move(gameState.player);

    if(col) {
        switch(col->type) {
            case OBJ_BREAKABLE: {
                if(key_helt(KC_S) || key_helt(KC_DOWN)) {
                    col->enabled = false;
                }
                else {
                    printf("Player dead :(\n");
                    return;
                }

                break;
            }
            case OBJ_DANGER: {
                printf("Player dead :(\n");
                return;
            }
            case OBJ_GOAL: {
                printf("Player won :)\n");
                return;
            }
        }
    }

    bool intersectsAnyWin = false;
    for(size_t i = 0; i < gameState.winCount; i++) {
        Window *w = gameState.windows[i];
        
        if(rects_intersect(gameState.player->loc, gameState.player->size, w->worldLoc, w->worldSize)) {
            intersectsAnyWin = true;
            break;
        }
    }

    if(!intersectsAnyWin) {
        printf("Player dead :(\n");
        return;
    }
}

int main(int argc, char *argv[]) {
    app_init(&tick);

    Level *level = level_create(
        (LevelWindow[]){
            LEVEL_WIN(-1.7f + 1.0f/2.0f, 0.9f - 0.5f/2.0f, 1.0f, 0.5f, 0xff0000, false, false, "Start"),
            LEVEL_WIN(-1.2f + 0.5f/2.0f, 0.4f - 1.15f/2.0f, 0.5f, 1.15f, 0x00ff00, false, false, "Drop"),
            LEVEL_WIN(1.0f, -0.4f, 0.4f, 0.7f, 0x0000ff, false, false, "Goal"),
            LEVEL_WIN(0.0f, 0.0f, 0.5f, 0.4f, 0xff00ff, true, false, "Lense")
        }, 4,
        (LevelObject[]){
            LEVEL_OBJ(-1.7f, 0.65f, 0.1f, 0.6f, OBJ_WALL),
            LEVEL_OBJ(-1.2f, 0.9f, 1.1f, 0.1f, OBJ_WALL),
            LEVEL_OBJ(-1.45f, 0.4f, 0.6f, 0.1f, OBJ_WALL),
            LEVEL_OBJ(-0.7f, 0.65f, 0.1f, 0.6f, OBJ_WALL),

            LEVEL_OBJ(-1.2f, -0.15f, 0.1f, 1.1f, OBJ_WALL),
            LEVEL_OBJ(-0.95f, -0.75f, 0.6f, 0.1f, OBJ_WALL),

            LEVEL_OBJ(0.05f, -0.775f, 1.4f, 0.05f, OBJ_DANGER),

            LEVEL_OBJ(1.0f, -0.75f, 0.5f, 0.1f, OBJ_WALL),
            LEVEL_OBJ(1.0f, -0.65f, 0.1f, 0.1f, OBJ_GOAL)
        }, 9,
        LEVEL_OBJ(-1.6f, 0.65f, 0.085f, 0.085f, OBJ_PLAYER),
        (LevelAura){.enabled = false}
    );

    level_load(level);
    app_run();

    return 0;
}