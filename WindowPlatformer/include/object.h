#pragma once

#include "vec2.h"
#include "SDL2/SDL.h"
#include "window.h"

typedef enum ObjectType {
    OBJ_WALL,
    OBJ_PLAYER,
    OBJ_DANGER,
    OBJ_BREAKABLE,
    OBJ_GOAL,
    OBJ_PORTAL
} ObjectType;

typedef struct Object {
    V2f loc, size;
    SDL_Rect output;
    ObjectType type;
} Object;


void obj_on_move(Object *obj);
void obj_on_resize(Object *obj);

void obj_draw(Object *obj, Window *win);