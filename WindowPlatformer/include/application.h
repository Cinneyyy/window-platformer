#pragma once

#include "number.h"
#include "window.h"
#include "SDL2/SDL.h"
#include "list.h"

extern const SDL_Rect DISPLAY_BOUNDS;
extern const i32 SCREEN_W, SCREEN_H, WH_DELTA;


void app_init(void (*tick)(f32));
void app_run(void);
void app_quit(void);

void app_set_title(const char *title);

void app_register_win(Window *win);
void app_deregister_win(Window *win);