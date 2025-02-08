#include "main_menu.h"
#include "stdio.h"
#include "window.h"
#include "color_palette.h"

static Window *mmWin;

void mm_load(void) {
    ColorPalette cpal = cpal_generate(0);
    mmWin = win_new("Main Menu Heheheha", (V2f){0, 0}, (V2f){1.6, 1.2}, true, false, &cpal);
}