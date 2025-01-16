#pragma once

typedef enum KeyCode KeyCode;

#include "SDL2/SDL.h"
#include "stdbool.h"

typedef enum KeyCode {
    KC_A,
    KC_B,
    KC_C,
    KC_D,
    KC_E,
    KC_F,
    KC_G,
    KC_H,
    KC_I,
    KC_J,
    KC_K,
    KC_L,
    KC_M,
    KC_N,
    KC_O,
    KC_P,
    KC_Q,
    KC_R,
    KC_S,
    KC_T,
    KC_U,
    KC_V,
    KC_W,
    KC_X,
    KC_Y,
    KC_Z,

    KC_0,
    KC_1,
    KC_2,
    KC_3,
    KC_4,
    KC_5,
    KC_6,
    KC_7,
    KC_8,
    KC_9,

    KC_F1,
    KC_F2,
    KC_F3,
    KC_F4,
    KC_F5,
    KC_F6,
    KC_F7,
    KC_F8,
    KC_F9,
    KC_F10,
    KC_F11,
    KC_F12,

    KC_LEFT,
    KC_RIGHT,
    KC_UP,
    KC_DOWN,

    KC_CTRL,
    KC_SHIFT,
    KC_ALT,

    KC_SPACE,
    KC_RETURN,
    KC_TAB,

    KC_LMB,
    KC_RMB,
    KC_MMB,

    KC_NONE
} KeyCode;


KeyCode sdl_key_to_keycode(SDL_KeyCode kc);
void input_init(void);
void input_handle_events(void);

bool key_helt(KeyCode kc);
bool key_down(KeyCode kc);
bool key_up(KeyCode kc);