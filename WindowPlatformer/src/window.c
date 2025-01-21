#include "renderer.h"
#include "window.h"
#include "world.h"
#include "number.h"
#include "color.h"
#include "game_state.h"
#include "stdio.h"
#include "SDL3/SDL.h"
#include "stdlib.h"

Window *win_new(char *title, V2f loc, V2f size, bool moveable, bool resizable, ColorPalette *colors) {
    Window *win = (Window*)malloc(sizeof(Window));

    win->worldLoc = loc;
    win->worldSize = size;
    win->screenLoc = world_point_to_screen((V2f){loc.x - size.x/2.0f, loc.y + size.y/2.0f});
    win->screenSize = world_scale_to_screen(size);
    win->sdlWin = SDL_CreateWindow(title, win->screenSize.x, win->screenSize.y, moveable ? 0 : SDL_WINDOW_BORDERLESS);
    SDL_SetWindowPosition(win->sdlWin, win->screenLoc.x, win->screenLoc.y);
    win->moveable = moveable;
    win->resizable = resizable;
    win->rend = rend_new(win);
    win->colors = colors;
    win->id = SDL_GetWindowID(win->sdlWin);

    SDL_SetWindowResizable(win->sdlWin, resizable);

    u8 bgr, bgg, bgb;
    col_get_rgb(colors->background, &bgr, &bgg, &bgb);
    SDL_SetRenderDrawColor(win->rend->sdlRend, bgr, bgg, bgb, 0xff);
    SDL_RenderClear(win->rend->sdlRend);
    SDL_RenderPresent(win->rend->sdlRend);

    return win;
}

void win_destroy(Window *win) {
    rend_destroy(win->rend);
    SDL_DestroyWindow(win->sdlWin);
    free(win);
}

void win_show(Window *win, bool show) {
    if(show) {
        SDL_ShowWindow(win->sdlWin);
    }
    else {
        SDL_HideWindow(win->sdlWin);
    }
}

void win_set_size(Window *win, V2f worldSize) {
    V2i screenSize = world_scale_to_screen(worldSize);
    SDL_SetWindowSize(win->sdlWin, screenSize.x, screenSize.y);
}

void win_set_loc(Window *win, V2f worldLoc) {
    V2i screenLoc = world_point_to_screen(worldLoc);
    SDL_SetWindowPosition(win->sdlWin, screenLoc.x, screenLoc.y);
}

void win_set_title(Window *win, const char *title) {
    SDL_SetWindowTitle(win->sdlWin, title);
}

void win_on_move(Window *win) {
    SDL_GetWindowPosition(win->sdlWin, &win->screenLoc.x, &win->screenLoc.y);
    win->worldLoc = world_point_from_screen(win->screenLoc);
}

void win_on_resize(Window *win) {
    SDL_GetWindowSize(win->sdlWin, &win->screenSize.x, &win->screenSize.y);
    win->worldSize = world_scale_from_screen(win->screenSize);
}

void win_redraw(Window *win) {
    u8 br, bg, bb;
    col_get_rgb(win->colors->background, &br, &bg, &bb);
    SDL_SetRenderDrawColor(win->rend->sdlRend, br, bg, bb, 0xff);
    SDL_RenderClear(win->rend->sdlRend);

    for(size_t i = 0; i < gameState.objCount; i++) {
        obj_draw(gameState.objects[i], win);
    }

    obj_draw(gameState.player, win);

    SDL_RenderPresent(win->rend->sdlRend);
}