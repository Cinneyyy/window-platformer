#pragma once

#include "number.h"
#include "SDL2/SDL.h"
#include "renderer.h"
#include "vec2.h"
#include "color_palette.h"

struct Renderer;
struct ColorPalette;

typedef struct Window {
    SDL_Window *sdlWin;
    struct Renderer *rend;
    bool moveable;
    V2f worldLoc, worldSize;
    V2i screenLoc, screenSize;
    struct ColorPalette colors;
} Window;


Window *win_new(char *title, V2f loc, V2f size, bool moveable, ColorPalette colors);
void win_destroy(Window *win);
void win_show(Window *win, bool show);

void win_set_size(Window *win, V2f worldSize);
void win_set_loc(Window *win, V2f worldLoc);

void win_set_title(Window *win, const char *title);