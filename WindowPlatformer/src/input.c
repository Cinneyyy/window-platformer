#include "input.h"
#include "SDL2/SDL.h"
#include "application.h"
#include "list.h"
#include "game_state.h"
#include "main_menu.h"
#include "stdio.h"

static const uint32_t KEY_COUNT = KC_NONE;
static const uint32_t MAX_CONCURRENT_KEYS = 8;

static List *additionPending, *removalPending;
static bool *keyState, *lastKeyState;


KeyCode sdl_key_to_keycode(SDL_KeyCode kc) {
    if(kc >= SDLK_a && kc <= SDLK_z) {
        return (KeyCode)(kc - SDLK_a + KC_A);
    }

    if(kc >= SDLK_0 && kc <= SDLK_9) {
        return (KeyCode)(kc - SDLK_0 + KC_0);
    }

    if(kc >= SDLK_F1 && kc <= SDLK_F12) {
        return (KeyCode)(kc - SDLK_F1 + KC_F1);
    }

    switch(kc) {
        case SDLK_LEFT: return KC_LEFT;
        case SDLK_RIGHT: return KC_RIGHT;
        case SDLK_UP: return KC_UP;
        case SDLK_DOWN: return KC_DOWN;

        case SDLK_LCTRL:
        case SDLK_RCTRL: return KC_CTRL;

        case SDLK_LSHIFT:
        case SDLK_RSHIFT: return KC_SHIFT;

        case SDLK_LALT:
        case SDLK_RALT: return KC_ALT;

        case SDLK_TAB: return KC_TAB;
        case SDLK_RETURN: return KC_RETURN;
        case SDLK_SPACE: return KC_SPACE;

        default: return KC_NONE;
    }
}

void input_init() {
    additionPending = list_new(MAX_CONCURRENT_KEYS);
    removalPending = list_new(MAX_CONCURRENT_KEYS);
    keyState = (bool*)calloc(KEY_COUNT, sizeof(bool));
    lastKeyState = (bool*)calloc(KEY_COUNT, sizeof(bool));
}

void input_handle_events() {
    SDL_Event evt;
    while(SDL_PollEvent(&evt)) {
        switch(evt.type) {
            case SDL_QUIT: {
                app_quit();
                return;
            };
            case SDL_WINDOWEVENT: {
                switch(evt.window.event) {
                    case SDL_WINDOWEVENT_CLOSE: {
                        if(gameState.loadedLevel) {
                            lvl_unload_current();
                            mm_load();
                        }
                        else {
                            app_quit();
                        }

                        return;
                    }
                }

                break;
            };
            case SDL_KEYDOWN: {
                if(evt.key.keysym.sym == SDLK_ESCAPE) {
                    app_quit();
                    return;
                }

                KeyCode key = sdl_key_to_keycode((SDL_KeyCode)evt.key.keysym.sym);

                if(key != KC_NONE && !keyState[(size_t)key]) {
                    list_append(additionPending, &key);
                }

                break;
            };
            case SDL_KEYUP: {
                KeyCode key = sdl_key_to_keycode((SDL_KeyCode)evt.key.keysym.sym);

                if(key != KC_NONE && keyState[(size_t)key]) {
                    list_append(removalPending, &key);
                }

                break;
            };
            case SDL_MOUSEBUTTONDOWN: {
                uint8_t mb = evt.button.button;
                KeyCode key = KC_NONE;

                switch(mb) {
                    case 0: key = KC_LMB; break;
                    case 1: key = KC_RMB; break;
                    case 2: key = KC_MMB; break;
                }

                if(key != KC_NONE && !keyState[(size_t)key]) {
                    list_append(additionPending, &key);
                }

                printf("Pressed mb: %i\n", mb);
                
                break;
            };
            case SDL_MOUSEBUTTONUP: {
                uint8_t mb = evt.button.button;
                KeyCode key = KC_NONE;

                switch(mb) {
                    case 0: key = KC_LMB; break;
                    case 1: key = KC_RMB; break;
                    case 2: key = KC_MMB; break;
                }
                
                if(key != KC_NONE && keyState[(size_t)key]) {
                    list_append(removalPending, &key);
                }

                printf("Released mb: %i\n", mb);
                
                break;
            }
        }
    }

    memcpy(lastKeyState, keyState, sizeof(bool) * KEY_COUNT);

    for(size_t i = 0; i < removalPending->count; i++) {
        keyState[*(size_t*)list_at(removalPending, i)] = false;
    }
    for(size_t i = 0; i < additionPending->count; i++) {
        keyState[*(size_t*)list_at(additionPending, i)] = true;
    }

    list_clear(removalPending);
    list_clear(additionPending);
}

bool key_helt(KeyCode key) {
    return keyState[(size_t)key];
}

bool key_down(KeyCode key) {
    return keyState[(size_t)key] && !lastKeyState[(size_t)key];
}

bool key_up(KeyCode key) {
    return !keyState[(size_t)key] && lastKeyState[(size_t)key];
}