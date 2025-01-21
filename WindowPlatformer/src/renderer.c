#include "renderer.h"
#include "SDL3/SDL.h"
#include "game_state.h"
#include "color.h"
#include "stdlib.h"

Renderer *rend_new(Window *win) {
    Renderer *rend = (Renderer*)malloc(sizeof(Renderer));

    rend->sdlRend = SDL_CreateRenderer(win->sdlWin, NULL);
    
    return rend;
}

void rend_destroy(Renderer *rend) {
    SDL_DestroyRenderer(rend->sdlRend);
    free(rend);
}