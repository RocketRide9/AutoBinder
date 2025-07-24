namespace KhronosRegisty;

public class Type
{
    /*
        input is e.g. 'typedef unsigned int GLenum;'
        or 'typedef khronos_int8_t GLbyte;' (depends on previously created one)
        or 'typedef struct __GLsync *GLsync;' (pointer to something)
        or 'typedef void (*GLDEBUGPROC)(GLenum source ...);' (delegate)
        or '#ifdef __APPLE__
            typedef void *GLhandleARB;
            #else
            typedef unsigned int GLhandleARB;
            #endif' (wtf?)
    */
    public Type(string input)
    {

    }
}
