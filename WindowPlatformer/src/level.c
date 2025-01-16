#include "level.h"
#include "game_state.h"
#include "window.h"
#include "stdio.h"

void lvl_load(Level *level) {
    if(gameState.loadedLevel) {
        return; // TODO: error logging
    }

    gameState.loadedLevel = level;

    gameState.winCount = level->windowCount;
    gameState.windows = malloc(level->windowCount * sizeof(Window*));

    for(size_t i = 0; i < level->windowCount; i++) {
        LevelWindow lWin = level->windows[i];
        Window *win = win_new(lWin.title, lWin.loc, lWin.size, lWin.moveable, lWin.resizable, lWin.colors);
        gameState.windows[i] = win;
    }

    gameState.objCount = level->objectCount;
    gameState.objects = malloc(level->objectCount * sizeof(Object*));

    for(size_t i = 0; i < level->objectCount; i++) {
        Object *obj = obj_new(level->objects[i]);
        gameState.objects[i] = obj;
        obj_on_move(obj);
        obj_on_resize(obj);
    }

    gameState.player = obj_new(level->player);
    obj_on_move(gameState.player);
    obj_on_resize(gameState.player);
}

void lvl_unload_current(void) {
    if(!gameState.loadedLevel) {
        return; // TODO: error logging
    }

    for(size_t i = 0; i < gameState.objCount; i++) {
        obj_destroy(gameState.objects[i]);
    }
    
    free(gameState.objects);

    for(size_t i = 0; i < gameState.winCount; i++) {
        win_destroy(gameState.windows[i]);
    }

    free(gameState.windows);

    obj_destroy(gameState.player);
    gameState.loadedLevel = NULL;
}