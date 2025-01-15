#include "level.h"
#include "game_state.h"
#include "window.h"

void lvl_load(Level *level) {
    if(gameState.loadedLevel) {
        return; // TODO: error logging
    }

    gameState.loadedLevel = level;

    gameState.winCount = level->windowCount;
    gameState.windows = malloc(level->windowCount * sizeof(Window**));

    for(size_t i = 0; i < level->windowCount; i++) {
        LevelWindow lWin = level->windows[i];
        Window *win = win_new(lWin.title, lWin.loc, lWin.size, lWin.moveable, lWin.colors); 

        gameState.windows[i] = win;
    }
}

void lvl_unload_current(void) {
}