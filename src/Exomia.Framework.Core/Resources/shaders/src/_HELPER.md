## DEBUG prints
```glsl
#extension GL_EXT_debug_printf : enable
#ifdef GL_EXT_debug_printf
    debugPrintfEXT("gl_VertexIndex = %i", gl_VertexIndex); 
#endif
```