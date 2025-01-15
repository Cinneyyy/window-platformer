#include "renderer.h"
#include "SDL2/SDL.h"

Renderer *rend_new(Window *win) {
    Renderer *rend = (Renderer*)malloc(sizeof(Renderer));

    rend->sdlRend = SDL_CreateRenderer(win->sdlWin, -1, SDL_RENDERER_ACCELERATED);
    
    return rend;
}

void rend_destroy(Renderer *rend) {
    SDL_DestroyRenderer(rend->sdlRend);
    free(rend);
}

void rend_render(Window *win) {
    
}