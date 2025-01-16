#include "SDL2/SDL.h"
#include "application.h"
#include "level.h"
#include "stdlib.h"
#include "stdio.h"
#include "renderer.h"
#include "game_state.h"
#include "math.h"

void tick(float dt) {
    gameState.player->loc.y = 0.75f * sinf(totalTime);
    obj_on_move(gameState.player);
}

int main(int argc, char *argv[]) {
    app_init(&tick);

    Level level = (Level){
        .windowCount = 1,
        .windows = (LevelWindow*)malloc(1 * sizeof(LevelWindow)),
        .objectCount = 0,
        .objects = NULL,
        .player = (LevelObject){
            .loc = {0, 0},
            .size = {0.085f, 0.085f},
            .type = OBJ_PLAYER
        },
        .aura = {
            .enabled = false
        }
    };

    level.windows[0] = (LevelWindow){
        .loc = {0, 0},
        .size = {1, 1},
        .moveable = true,
        .resizable = true,
        .title = "mmm",
        .colors = cpal_alloc(0xff0000)
    };

    lvl_load(&level);

    app_run();
    return 0;
}