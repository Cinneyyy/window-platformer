#include "world.h"
#include "math.h"
#include "application.h"

V2i world_point_to_screen(V2f point) {
    return (V2i){
        (i32)floorf((point.x + 1.0f) / 2.0f * SCREEN_H + WH_DELTA),
        (i32)floorf((-point.y + 1.0f) / 2.0f * SCREEN_H)
    };
}

V2i world_scale_to_screen(V2f scale) {
    return (V2i){
        (i32)floorf(scale.x / 2.0f * SCREEN_H),
        (i32)floorf(scale.y / 2.0f * SCREEN_H)
    };
}