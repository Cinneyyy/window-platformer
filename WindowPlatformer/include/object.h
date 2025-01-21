#pragma once

typedef enum ObjectType {
    OBJ_WALL,
    OBJ_PLAYER,
    OBJ_DANGER,
    OBJ_BREAKABLE,
    OBJ_GOAL,
    OBJ_PORTAL
} ObjectType;

typedef struct Object Object;

#include "vec2.h"
#include "SDL3/SDL.h"
#include "window.h"
#include "level.h"


typedef struct Object {
    V2f loc, size;
    SDL_FRect output;
    ObjectType type;
    bool enabled;
} Object;


Object *obj_new(LevelObject lObj);
void obj_destroy(Object *obj);

void obj_on_move(Object *obj);
void obj_on_resize(Object *obj);

void obj_draw(Object *obj, Window *win);