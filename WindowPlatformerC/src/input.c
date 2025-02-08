#include "input.h"
#include "SDL3/SDL.h"
#include "application.h"
#include "list.h"
#include "main_menu.h"
#include "stdio.h"
#include "stdlib.h"
#include "game_state.h"

static const u32 KEY_COUNT = KC_NONE;


static SDL_Scancode keycode_to_sdl_scancode(KeyCode kc) {
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


// For debug purposes, editor greys out the code otherwise
//#define WINDOWS_BUILD
//#define LINUX_BUILD
//#define MAC_BUILD

#ifdef WINDOWS_BUILD
#include "windows.h"

static int kc_to_vk(KeyCode kc) {
    const int VK_A = 0x41;
    const int VK_1 = 0x31;
    const int VK_0 = 0x30;

    if(kc >= KC_A && kc <= KC_Z) {
        return kc - KC_A + VK_A;
    }

    if(kc >= KC_1 && kc <= KC_9) {
        return kc - KC_0 + VK_1;
    }

    if(kc >= KC_F1 && kc <= KC_F12) {
        return kc - KC_F1 + VK_F1;
    }

    switch(kc) {
        case KC_0: return VK_0;

        case KC_LEFT: return VK_LEFT;
        case KC_RIGHT: return VK_RIGHT;
        case KC_UP: return VK_UP;
        case KC_DOWN: return VK_DOWN;

        case KC_CTRL: return VK_LCONTROL;
        case KC_SHIFT: return VK_LSHIFT;
        case KC_ALT: return VK_LMENU;

        case KC_TAB: return VK_TAB;
        case KC_RETURN: return VK_RETURN;
        case KC_SPACE: return VK_SPACE;

        default: return 0;
    }
}


void input_init(void) {
}

bool key_held(KeyCode kc) {
    return GetAsyncKeyState(kc_to_vk(kc)) < 0;
}

bool key_down(KeyCode kc) {
    return GetAsyncKeyState(kc_to_vk(kc)) & 0x0001 != 0;
}

bool key_up(KeyCode kc) {
    return false;
}
#endif

// TODO: impl
#ifdef LINUX_BUILD

#define KeyCode __LKC
#include "X11/Xlib.h"
#include "X11/keysym.h"
#undef KeyCode

static Display *display;
static __LKC spaceKey;


void input_init(void) {
    display = XOpenDisplay(NULL);
}

bool key_held(KeyCode kc) {
    return GetAsyncKeyState(kc_to_vk(kc)) < 0;
}

bool key_down(KeyCode kc) {
    return GetAsyncKeyState(kc_to_vk(kc)) & 0x0001 != 0;
}

bool key_up(KeyCode kc) {
    return false;
}
#endif

// TODO: impl
#ifdef MAC_BUILD
#endif