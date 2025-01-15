#include "application.h"
#include "stdio.h"
#include "SDL2/SDL.h"
#include "input.h"
#include "game_state.h"
#include "level.h"

const SDL_Rect DISPLAY_BOUNDS;
const i32 SCREEN_W, SCREEN_H, WH_DELTA;

static bool initiated = false;
static bool isRunning = false;
static void (*tickFpt)(f32);


void app_init(void (*tick)(f32)) {
    if(initiated) {
        printf("Cannot initiate application multiple times.\n");
        return;
    }

    SDL_SetHint(SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");

    if(SDL_Init(SDL_INIT_VIDEO) < 0) {
        printf("Failed to initialize SDL: %s\n", SDL_GetError());
        return;
    }

    SDL_GetDisplayBounds(0, (SDL_Rect*)&DISPLAY_BOUNDS);
    *(i32*)&SCREEN_W = DISPLAY_BOUNDS.w;
    *(i32*)&SCREEN_H = DISPLAY_BOUNDS.h;
    *(i32*)&WH_DELTA = (SCREEN_W - SCREEN_H) / 2;

    tickFpt = tick;

    input_init();

    initiated = true;
}

void app_run(void) {
    if(isRunning) {
        printf("Cannot start main loop, while it is already active.\n");
        return;
    }

    isRunning = true;

    u32 lastFrame = SDL_GetTicks();
    f32 deltaTime, totalTime;

    while(isRunning) {
        input_handle_events();

        u32 now = SDL_GetTicks();
        deltaTime = (f32)(now - lastFrame) / 1000.0f;
        totalTime = (f32)SDL_GetTicks() / 1000.0f;
        lastFrame = now;

        tickFpt(deltaTime);

        SDL_Delay(1);
    }

    lvl_unload_current();

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