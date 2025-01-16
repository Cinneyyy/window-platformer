#include "utils.h"

inline f32 clampf(f32 v, f32 min, f32 max) {
    if(v <= min) {
        return min;
    }
    else if(v >= max) {
        return max;
    }
    else {
        return v;
    }
}

inline i32 clampi(i32 v, i32 min, i32 max) {
    if(v <= min) {
        return min;
    }
    else if(v >= max) {
        return max;
    }
    else {
        return v;
    }
}