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

typedef struct LevelWindow {
    V2f loc, size;
    bool moveable, resizable;
    ColorPalette *colors;
    char *title;
} LevelWindow;

typedef struct LevelObject {
    V2f loc, size;
    ObjectType type;
} LevelObject;

typedef struct LevelAura {
    bool enabled;
    f32 size;
    ColorPalette colors;
} LevelAura;

typedef struct Level {
    size_t windowCount;
    LevelWindow *windows;
    size_t objectCount;
    LevelObject *objects;
    LevelObject player;
    LevelAura aura;
} Level;


void lvl_load(Level *level);
void lvl_unload_current(void);