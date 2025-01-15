#pragma once

#include "window.h"
#include "SDL2/SDL.h"

typedef struct Renderer {
    SDL_Renderer *sdlRend;
} Renderer;

typedef struct Window Window;


Renderer *rend_new(Window *win);
void rend_destroy(Renderer *rend);

void rend_render(Window *win);