#include "renderer.h"
#include "window.h"
#include "world.h"
#include "number.h"
#include "color.h"

Window *win_new(char *title, V2f loc, V2f size, bool moveable, ColorPalette colors) {
    Window *win = (Window*)malloc(sizeof(Window));

    win->worldLoc = loc;
    win->worldSize = size;
    win->screenLoc = world_point_to_screen(loc);
    win->screenSize = world_scale_to_screen(size);
    win->sdlWin = SDL_CreateWindow(title, win->screenLoc.x, win->screenLoc.y, win->screenSize.y, win->screenSize.y, moveable ? SDL_WINDOW_SHOWN : SDL_WINDOW_BORDERLESS);
    win->moveable = moveable;
    win->rend = rend_new(win);
    win->colors = colors;

    SDL_SetWindowResizable(win->sdlWin, SDL_FALSE);

    u8 bgr, bgg, bgb;
    col_get_rgb(colors.background, &bgr, &bgg, &bgb);
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