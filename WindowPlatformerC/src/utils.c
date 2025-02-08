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

inline f32 signf(f32 f) {
    if(f < 0.0f) {
        return -1.0f;
    }
    else if(f > 0.0f) {
        return 1.0f;
    }
    else {
        return 0.0f;
    }
}
inline i32 signi(i32 i) {
    if(i < 0) {
        return -1;
    }
    else if(i > 0) {
        return 1;
    }
    else {
        return 0;
    }
}

inline f32 absf(f32 f) {
    if(f < 0.0f) {
        return -f;
    }
    else {
        return f;
    }
}

inline f32 maxf(f32 a, f32 b) {
    if(a > b) {
        return a;
    }
    else {
        return b;
    }
}
inline f32 minf(f32 a, f32 b) {
    if(a > b) {
        return a;
    }
    else {
        return b;
    }
}
inline i32 maxi(i32 a, i32 b) {
    if(a > b) {
        return a;
    }
    else {
        return b;
    }
}
inline i32 mini(i32 a, i32 b) {
    if(a > b) {
        return a;
    }
    else {
        return b;
    }
}