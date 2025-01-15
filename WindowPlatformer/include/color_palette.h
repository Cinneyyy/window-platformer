#pragma once

#include "object.h"

typedef struct ColorPalette {
    u32 background, wall, player, danger, goal, portal, breakable;
} ColorPalette;


ColorPalette cpal_generate(u32 c);
u32 cpal_get_col(ColorPalette *pal, ObjectType objType);