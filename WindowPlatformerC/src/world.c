#include "world.h"
#include "math.h"
#include "application.h"
#include "stdio.h"

inline V2i world_point_to_screen(V2f point) {
    return (V2i){
        (i32)floorf((point.x + 1.0f) / 2.0f * SCREEN_H + WH_DELTA),
        (i32)floorf((-point.y + 1.0f) / 2.0f * SCREEN_H)
    };
}

inline V2i world_scale_to_screen(V2f scale) {
    return (V2i){
        (i32)floorf(scale.x / 2.0f * SCREEN_H),
        (i32)floorf(scale.y / 2.0f * SCREEN_H)
    };
}

inline V2f world_point_from_screen(V2i point) {
    return (V2f){
        (point.x - WH_DELTA) / SCREEN_H * 2.0f - 1.0f,
        -(point.y / SCREEN_H * 2.0f - 1.0f)
    };
}

inline V2f world_scale_from_screen(V2i scale) {
    return (V2f){
        scale.x / SCREEN_H * 2.0f,
        scale.y / SCREEN_H * 2.0f
    };
}