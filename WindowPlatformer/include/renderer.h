#pragma once

typedef struct Renderer Renderer;

#include "window.h"
#include "SDL2/SDL.h"

typedef struct Renderer {
    SDL_Renderer *sdlRend;
} Renderer;


Renderer *rend_new(Window *win);
void rend_destroy(Renderer *rend);