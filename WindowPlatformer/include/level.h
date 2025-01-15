#pragma once

#include "vec2.h"
#include "number.h"
#include "stdbool.h"
#include "color_palette.h"
#include "object.h"

typedef struct ColorPalette ColorPalette;

typedef enum ObjectType ObjectType;

typedef struct LevelWindow {
    const V2f loc, size;
    const bool moveable;
    const ColorPalette colors;
    const char *title;
} LevelWindow;

typedef struct LevelObject {
    const V2f loc, size;
    const ObjectType type;
} LevelObject;

typedef struct LevelAura {
    const bool enabled;
    const f32 size;
    const ColorPalette colors;
} LevelAura;

typedef struct Level {
    const size_t windowCount;
    const LevelWindow *windows;
    const size_t objectCount;
    const LevelObject *objects;
    const LevelObject player;
    const LevelAura aura;
} Level;


void lvl_load(Level *level);
void lvl_unload_current(void);