#pragma once

typedef struct GameState GameState;

#include "level.h"
#include "object.h"
#include "window.h"

typedef struct GameState {
    size_t objCount;
    Object **objects;
    size_t winCount;
    Window **windows;
    Level *loadedLevel;
    Object *player;
} GameState;


extern GameState gameState;
extern bool isLevelLoading;