namespace src;

#pragma warning disable IDE0018 // Inline variable declaration
public static class Renderer
{
    public static void DrawWindow(Window win)
    {
        u8 r, g, b;

        win.colors.background.GetRgb(out r, out g, out b);
        SDL_SetRenderDrawColor(win.sdlRend, r, g, b, 0xff).ThrowSdlError();
        SDL_RenderClear(win.sdlRend).ThrowSdlError();

        foreach(GameObject obj in GameObjectManager.objs)
        {
            win.colors[obj.type].GetRgb(out r, out g, out b);
            SDL_SetRenderDrawColor(win.sdlRend, r, g, b, 0xff).ThrowSdlError();

            SDL_Rect output = obj.output;
            output.x -= win.screenLoc.x;
            output.y -= win.screenLoc.y;
            SDL_RenderFillRect(win.sdlRend, ref output).ThrowSdlError();
        }

        SDL_RenderPresent(win.sdlRend);
    }
}