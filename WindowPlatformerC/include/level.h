#pragma once

typedef struct LevelWindow LevelWindow;
typedef struct LevelObject LevelObject;
typedef struct LevelAura LevelAura;
typedef struct Level Level;

#include "vec2.h"
#include "number.h"
#include "stdbool.h"
#include "color_palette.h"
#include "object.h"

#define LEVEL_OBJ(_x, _y, _w, _h, _type) (LevelObject){ \
    .loc = (V2f){(_x), (_y)}, \
    .size = (V2f){(_w), (_h)}, \
    .type = (_type) \
}
#define LEVEL_WIN(_x, _y, _w, _h, _color, _moveable, _resizable, _title) (LevelWindow){ \
    .loc = (V2f){(_x), (_y)}, \
    .size = (V2f){(_w), (_h)}, \
    .color = (_color), \
    .moveable = (_moveable), \
    .resizable = (_resizable), \
    .title = (_title) \
}

typedef struct LevelWindow {
    V2f loc, size;
    bool moveable, resizable;
    u32 color;
    char *title;
} LevelWindow;

typedef struct LevelObject {
    V2f loc, size;
    ObjectType type;
} LevelObject;

typedef struct LevelAura {
    bool enabled;
    f32 size;
    u32 color;
} LevelAura;

typedef struct Level {
    size_t windowCount;
    LevelWindow *windows;
    size_t objectCount;
    LevelObject *objects;
    LevelObject player;
    LevelAura aura;
} Level;


void level_load(Level *level);
void level_unload_current(void);

Level *level_create(LevelWindow *windows, size_t windowCount, LevelObject *objects, size_t objectCount, LevelObject player, LevelAura aura);