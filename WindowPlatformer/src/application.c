#include "application.h"
#include "stdio.h"
#include "SDL3/SDL.h"
#include "input.h"
#include "game_state.h"
#include "level.h"
#include "main_menu.h"

SDL_Rect DISPLAY_BOUNDS;
i32 SCREEN_W, SCREEN_H, WH_DELTA;
f32 deltaTime, totalTime;

static bool initiated = false;
static bool isRunning = false;
static void (*tickFpt)(f32);
static u32 lastFrame;


static void handle_frame();

static bool filter_window_events(void *userdata, SDL_Event *evt) {
    if(evt->type == SDL_EVENT_WINDOW_EXPOSED) {
        printf("E");
    }

    if(isLevelLoading) {
        return true;
    }

    if(evt->type >= SDL_EVENT_WINDOW_FIRST && evt->type <= SDL_EVENT_WINDOW_LAST) {
        return true;
    }

    switch(evt->window.type) {
        case SDL_EVENT_WINDOW_CLOSE_REQUESTED: {
            app_quit(); // TODO: load main menu instead of quitting, if it's not already loaded
            break;
        }
        case SDL_EVENT_WINDOW_RESIZED:
        case SDL_EVENT_WINDOW_MOVED: {
            Window *win = NULL;
            for(size_t i = 0; i < gameState.winCount; i++) {
                if(gameState.windows[i]->id == evt->window.windowID) {
                    win = gameState.windows[i];
                    break;
                }
            }

            if(!win) {
                break;
            }

            if(evt->window.type == SDL_EVENT_WINDOW_RESIZED) {
                win_on_resize(win);
            }
            else if(evt->window.type == SDL_EVENT_WINDOW_MOVED) {
                win_on_move(win);
            }
            break;
        }
        case SDL_EVENT_WINDOW_EXPOSED: {
            handle_frame();
            break;
        }
        default: return true;
    }

    return false;
}

static void handle_frame() {
    input_handle_events();

    u32 now = SDL_GetTicks();
    deltaTime = (f32)(now - lastFrame) / 1000.0f;
    totalTime = (f32)SDL_GetTicks() / 1000.0f;
    lastFrame = now;

    tickFpt(deltaTime);

    if(gameState.loadedLevel) {
        for(size_t i = 0; i < gameState.loadedLevel->windowCount; i++) {
            win_redraw(gameState.windows[i]);
        }
    }

    SDL_Delay(1);
}


void app_init(void (*tick)(f32)) {
    if(initiated) {
        printf("Cannot initiate application multiple times.\n");
        return;
    }

    SDL_SetHint(SDL_HINT_QUIT_ON_LAST_WINDOW_CLOSE, "0");

    if(!SDL_Init(SDL_INIT_VIDEO)) {
        printf("Failed to initialize SDL: %s\n", SDL_GetError());
        return;
    }

    SDL_Point zero_zero = {.x = 0, .y = 0};
    SDL_DisplayID displayId = SDL_GetDisplayForPoint(&zero_zero);
    if(!SDL_GetDisplayBounds(displayId, &DISPLAY_BOUNDS)) {
        printf(SDL_GetError());
        return;
    }
    SCREEN_W = DISPLAY_BOUNDS.w;
    SCREEN_H = DISPLAY_BOUNDS.h;
    WH_DELTA = (SCREEN_W - SCREEN_H) / 2;

    tickFpt = tick;
    input_init();

    SDL_AddEventWatch(&filter_window_events, NULL);

    initiated = true;
}

void app_run(void) {
    if(isRunning) {
        printf("Cannot start main loop, while it is already active.\n");
        return;
    }

    isRunning = true;

    lastFrame = SDL_GetTicks();
    deltaTime = 0.0f;
    totalTime = 0.0f;

    while(isRunning) {
        handle_frame();
    }

    if(gameState.loadedLevel) {
        level_unload_current();
    }

    SDL_Quit();
}

void app_quit(void) { 
    if(!isRunning) {
        printf("Cannot quit main loop, while it is not running.\n");
        return;
    }

    isRunning = false;
}

void app_set_title(const char *title) {
    SDL_SetHint(SDL_HINT_APP_NAME, title);
}