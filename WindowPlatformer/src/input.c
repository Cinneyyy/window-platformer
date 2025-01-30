#include "input.h"
#include "SDL3/SDL.h"
#include "application.h"
#include "list.h"
#include "main_menu.h"
#include "stdio.h"
#include "stdlib.h"
#include "game_state.h"

static const u32 KEY_COUNT = KC_NONE;
static const u32 MAX_CONCURRENT_KEYS = 8;

static List *additionPending, *removalPending;
static bool *keyState, *lastKeyState;


KeyCode sdl_key_to_keycode(u32 kc) {
    if(kc >= SDLK_A && kc <= SDLK_Z) {
        return (KeyCode)(kc - SDLK_A + KC_A);
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

SDL_Scancode keycode_to_sdl_scancode(KeyCode kc) {
    if(kc >= KC_A && kc <= KC_Z) {
        return (SDL_Scancode)(kc - KC_A + SDL_SCANCODE_A);
    }

    if(kc >= KC_0 && kc <= KC_9) {
        return (SDL_Scancode)(kc - KC_0 + SDL_SCANCODE_0);
    }

    if(kc >= KC_F1 && kc <= KC_F12) {
        return (SDL_Scancode)(kc - KC_F1 + SDL_SCANCODE_F1);
    }

    switch(kc) {
        case KC_LEFT: return SDL_SCANCODE_LEFT;
        case KC_RIGHT: return SDL_SCANCODE_RIGHT;
        case KC_UP: return SDL_SCANCODE_UP;
        case KC_DOWN: return SDL_SCANCODE_DOWN;

        case KC_CTRL: return SDL_SCANCODE_LCTRL;
        case KC_SHIFT: return SDL_SCANCODE_LSHIFT;
        case KC_ALT: return SDL_SCANCODE_LALT;

        case KC_TAB: return SDL_SCANCODE_TAB;
        case KC_RETURN: return SDL_SCANCODE_RETURN;
        case KC_SPACE: return SDL_SCANCODE_SPACE;

        default: return SDL_SCANCODE_UNKNOWN;
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
            case SDL_EVENT_WINDOW_CLOSE_REQUESTED:
            case SDL_EVENT_QUIT: {
                app_quit();
                return;
            };
            case SDL_EVENT_KEY_DOWN: {
                if(evt.key.key == SDLK_ESCAPE) {
                    app_quit();
                    return;
                }

                KeyCode key = sdl_key_to_keycode(evt.key.key);

                if(key != KC_NONE && !keyState[(size_t)key]) {
                    list_append(additionPending, (void*)key);
                }

                break;
            };
            case SDL_EVENT_KEY_UP: {
                KeyCode key = sdl_key_to_keycode(evt.key.key);

                if(key != KC_NONE && keyState[(size_t)key]) {
                    list_append(removalPending, (void*)key);
                }

                break;
            };
            case SDL_EVENT_MOUSE_BUTTON_DOWN: {
                uint8_t mb = evt.button.button;
                KeyCode key = KC_NONE;

                switch(mb) {
                    case 1: key = KC_LMB; break;
                    case 2: key = KC_MMB; break;
                    case 3: key = KC_RMB; break;
                }

                if(key != KC_NONE && !keyState[(size_t)key]) {
                    list_append(additionPending, (void*)key);
                }
                
                break;
            };
            case SDL_EVENT_MOUSE_BUTTON_UP: {
                uint8_t mb = evt.button.button;
                KeyCode key = KC_NONE;

                switch(mb) {
                    case 1: key = KC_LMB; break;
                    case 2: key = KC_MMB; break;
                    case 3: key = KC_RMB; break;
                }
                
                if(key != KC_NONE && keyState[(size_t)key]) {
                    list_append(removalPending, (void*)key);
                }
                
                break;
            }
        }
    }
}

void input_advance(void) {
    memcpy(lastKeyState, keyState, sizeof(bool) * KEY_COUNT);

    for(size_t i = 0; i < removalPending->count; i++) {
        keyState[(size_t)list_at(removalPending, i)] = false;
    }
    for(size_t i = 0; i < additionPending->count; i++) {
        keyState[(size_t)list_at(additionPending, i)] = true;
    }

    list_clear(removalPending);
    list_clear(additionPending);
}

bool key_helt(KeyCode key) {
    SDL_PumpEvents();
    const bool *state = SDL_GetKeyboardState(NULL);
    return state[keycode_to_sdl_scancode(key)];
    return keyState[(size_t)key];
}

bool key_down(KeyCode key) {
    SDL_PumpEvents();
    const bool *state = SDL_GetKeyboardState(NULL);
    return state[keycode_to_sdl_scancode(key)];
    return keyState[(size_t)key] && !lastKeyState[(size_t)key];
}

bool key_up(KeyCode key) {
    SDL_PumpEvents();
        const bool *state = SDL_GetKeyboardState(NULL);
    return !state[keycode_to_sdl_scancode(key)];
        return !keyState[(size_t)key] && lastKeyState[(size_t)key];
}