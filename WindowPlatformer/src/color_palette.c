#include "color.h"
#include "color_palette.h"

ColorPalette cpal_generate(u32 c) {
    return (ColorPalette){
        .background = col_multiply(c, 0.225f), // Multiply by 0.225
        .wall = col_multiply(c, 0.5f), // Multiply by 0.5
        .danger = col_invert(c), // Invert
        .player = col_mix_linear(c, WHITE), // Mix with white
        .goal = 0xffffb300u, // Gold
        .breakable = col_multiply(col_mix_linear(c, col_gray(0xaa)), 0.35f), // Mix with #aaaaaa, then multiply by 0.35
        .portal = 0xff008000 // Dark green
    };
}

u32 cpal_get_col(ColorPalette *pal, ObjectType objType) {
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