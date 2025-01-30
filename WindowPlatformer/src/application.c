#include "application.h"
#include "stdio.h"
#include "SDL3/SDL.h"
#include "input.h"
#include "game_state.h"
#include "level.h"
#include "main_menu.h"
#include "pthread.h"

SDL_Rect DISPLAY_BOUNDS;
i32 SCREEN_W, SCREEN_H, WH_DELTA;
f32 deltaTime, totalTime;

static bool initiated = false;
static bool isRunning = false;
static void (*tickFpt)(f32);
static u32 lastFrame;
static pthread_t *gameThread;


static void *game_thread_run(void *data) {
    i32 fpsCounter = 0;

    while(isRunning) {
        u32 now = SDL_GetTicks();
        deltaTime = (f32)(now - lastFrame) / 1000.0f;
        totalTime = (f32)now / 1000.0f;
        lastFrame = now;

        tickFpt(deltaTime);

        if(gameState.loadedLevel) {
            for(size_t i = 0; i < gameState.loadedLevel->windowCount; i++) {
                win_redraw(gameState.windows[i]);
            }
        }

        SDL_Delay(1);
    }
}

static Window *get_window_from_handle(size_t hwnd) {
    printf("Trying to get window %i\n", hwnd);
    for(size_t i = 0; i < gameState.winCount; i++) {
        if(gameState.windows[i] && gameState.windows[i]->id == hwnd) {
            printf("Found window: %i %i %i %i", i, hwnd, gameState.windows[i]->screenSize.x, gameState.windows[i]->screenSize.y);
            return gameState.windows[i];
        }
    }

    return NULL;
}

static bool evt_watch(void *data, SDL_Event *evt) {
    switch(evt->type) {
        case SDL_EVENT_WINDOW_CLOSE_REQUESTED:
        case SDL_EVENT_QUIT: {
            Window *win = get_window_from_handle(evt->window.windowID);

            if(!win) {
                app_quit();
                break;
            }

            if(gameState.loadedLevel) {
                level_unload_current();
                mm_load();
            }
            else {
                app_quit();
            }

            break;
        }
        case SDL_EVENT_WINDOW_RESIZED:
        case SDL_EVENT_WINDOW_MOVED: {
            Window *win = get_window_from_handle(evt->window.windowID);

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
        default: break;
    }

    return false;
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

    SDL_AddEventWatch(&evt_watch, NULL);

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

    pthread_create(gameThread, NULL, &game_thread_run, NULL);

    while(isRunning) {
        SDL_Event evt;
        while(SDL_PollEvent(&evt)) {
            continue;
        }
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