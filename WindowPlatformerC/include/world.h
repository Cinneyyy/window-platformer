#pragma once

#include "vec2.h"

V2i world_point_to_screen(V2f point);
V2i world_scale_to_screen(V2f scale);

V2f world_point_from_screen(V2i point);
V2f world_scale_from_screen(V2i scale);