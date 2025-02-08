#pragma once

typedef struct Window Window;

#include "number.h"
#include "SDL3/SDL.h"
#include "renderer.h"
#include "vec2.h"
#include "color_palette.h"

typedef struct Window {
    SDL_Window *sdlWin;
    Renderer *rend;
    bool moveable, resizable;
    V2f worldLoc, worldSize;
    V2i screenLoc, screenSize;
    ColorPalette *colors;
    u32 id;
} Window;


Window *win_new(char *title, V2f loc, V2f size, bool moveable, bool resizable, ColorPalette *colors);
void win_destroy(Window *win);
void win_show(Window *win, bool show);

void win_set_size(Window *win, V2f worldSize);
void win_set_loc(Window *win, V2f worldLoc);

void win_set_title(Window *win, const char *title);

void win_on_move(Window *win);
void win_on_resize(Window *win);

void win_redraw(Window *win);