#include "object.h"
#include "world.h"

void obj_on_move(Object *obj) {
    V2i sLoc = world_point_to_screen(obj->loc);
    obj->output.x = sLoc.x;
    obj->output.y = sLoc.y;
}

void obj_on_resize(Object *obj) {
    V2i sSize = world_scale_to_screen(obj->size);
    obj->output.w = sSize.x;
    obj->output.h = sSize.y;
}

void obj_draw(Object *obj, Window *win) {
    SDL_Renderer *sdlRend = win->rend->sdlRend;

    u8 r, g, b;
    col_get_rgb(cpal_get_col(&win->colors, obj->type), &r, &g, &b);

    SDL_SetRenderDrawColor(sdlRend, r, g, b, 0xff);
    SDL_RenderDrawRect(sdlRend, &obj->output);
}