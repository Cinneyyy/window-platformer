#include "color.h"
#include "color_palette.h"
#include "object.h"

static void populate(ColorPalette *cpal, u32 c) {
    cpal->background = col_multiply(c, 0.225f); // Multiply by 0.225
    cpal->wall = col_multiply(c, 0.5f); // Multiply by 0.5
    cpal->danger = col_invert(c); // Invert
    cpal->player = col_mix_linear(c, WHITE); // Mix with white
    cpal->goal = 0xffffb300; // Gold
    cpal->breakable = col_multiply(col_mix_linear(c, col_gray(0xaa)), 0.35f); // Mix with #aaaaaa, then multiply by 0.35
    cpal->portal = 0xff008000; // Dark green
}


ColorPalette cpal_generate(u32 c) {
    ColorPalette cpal;
    populate(&cpal, c);
    return cpal;
}

ColorPalette *cpal_alloc(u32 c) {
    ColorPalette *cpal = (ColorPalette*)malloc(sizeof(ColorPalette));
    populate(cpal, c);
    return cpal;
}

u32 cpal_get_col(const ColorPalette *pal, ObjectType objType) {
    switch(objType) {
        case OBJ_WALL: return pal->wall;
        case OBJ_PLAYER: return pal->player;
        case OBJ_DANGER: return pal->danger;
        case OBJ_GOAL: return pal->goal;
        case OBJ_BREAKABLE: return pal->breakable;
        case OBJ_PORTAL: return pal->portal;
        default: return BLACK;
    }
}