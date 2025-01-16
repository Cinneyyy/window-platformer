#include "object.h"
#include "world.h"
#include "color.h"
#include "stdio.h"

Object *obj_new(LevelObject lObj) {
    Object *obj = (Object*)malloc(sizeof(Object));

    obj->loc = lObj.loc;
    obj->size = lObj.size;
    obj->type = lObj.type;
    obj->enabled = true;

    return obj;
}

void obj_destroy(Object *obj) {
    free(obj);
}

void obj_on_move(Object *obj) {
    V2i sLoc = world_point_to_screen((V2f){obj->loc.x - obj->size.x/2.0f, obj->loc.y + obj->size.y/2.0f});
    obj->output.x = sLoc.x;
    obj->output.y = sLoc.y;
}

void obj_on_resize(Object *obj) {
    V2i sSize = world_scale_to_screen(obj->size);
    obj->output.w = sSize.x;
    obj->output.h = sSize.y;
}

void obj_draw(Object *obj, Window *win) {
    if(!obj->enabled) {
        return;
    }

    SDL_Renderer *sdlRend = win->rend->sdlRend;

    u8 r, g, b;
    col_get_rgb(cpal_get_col(win->colors, obj->type), &r, &g, &b);

    SDL_Rect rect = obj->output;
    rect.x -= win->screenLoc.x;
    rect.y -= win->screenLoc.y;

    SDL_SetRenderDrawColor(sdlRend, r, g, b, 0xff);
    SDL_RenderFillRect(sdlRend, &rect);
}