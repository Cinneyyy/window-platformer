#include "level.h"
#include "game_state.h"
#include "window.h"
#include "stdio.h"
#include "stdlib.h"

void level_load(Level *level) {
    isLevelLoading = true;

    if(gameState.loadedLevel) {
        return; // TODO: error logging
    }

    gameState.loadedLevel = level;

    gameState.winCount = level->windowCount;
    gameState.windows = (Window**)malloc(level->windowCount * sizeof(Window*));

    for(size_t i = 0; i < level->windowCount; i++) {
        LevelWindow lWin = level->windows[i];
        Window *win = win_new(lWin.title, lWin.loc, lWin.size, lWin.moveable, lWin.resizable, cpal_alloc(lWin.color));
        gameState.windows[i] = win;
    }

    gameState.objCount = level->objectCount;
    gameState.objects = (Object**)malloc(level->objectCount * sizeof(Object*));

    for(size_t i = 0; i < level->objectCount; i++) {
        Object *obj = obj_new(level->objects[i]);
        gameState.objects[i] = obj;
        obj_on_move(obj);
        obj_on_resize(obj);
    }

    gameState.player = obj_new(level->player);
    obj_on_move(gameState.player);
    obj_on_resize(gameState.player);

    isLevelLoading = false;
}

void level_unload_current(void) {
    if(!gameState.loadedLevel) {
        return; // TODO: error logging
    }

    for(size_t i = 0; i < gameState.objCount; i++) {
        obj_destroy(gameState.objects[i]);
    }
    
    free(gameState.objects);

    for(size_t i = 0; i < gameState.winCount; i++) {
        free(gameState.windows[i]->colors);
        win_destroy(gameState.windows[i]);
    }

    free(gameState.windows);

    obj_destroy(gameState.player);
    gameState.loadedLevel = NULL;
}

Level *level_create(LevelWindow *windows, size_t windowCount, LevelObject *objects, size_t objectCount, LevelObject player, LevelAura aura) {
    Level *level = (Level*)malloc(sizeof(Level));

    level->windowCount = windowCount;
    level->windows = (LevelWindow*)malloc(windowCount * sizeof(LevelWindow));
    for(size_t i = 0; i < windowCount; i++) {
        level->windows[i] = windows[i];
    }

    level->objectCount = objectCount;
    level->objects = (LevelObject*)malloc(objectCount * sizeof(LevelObject));
    for(size_t i = 0; i < objectCount; i++) {
        level->objects[i] = objects[i];
    }

    level->player = player;
    level->aura = aura;
    return level;
}