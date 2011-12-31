using gl = Tao.OpenGl.Gl;

namespace GtkGL {
	
    public class Triangle : GtkGL.GLObjectBase, GtkGL.IGLObject{
		protected override void DrawObject()
		{ 
			gl.glBegin(gl.GL_TRIANGLES);					// Drawing Using Triangles
				gl.glVertex3f( 0.0f, 1.0f, 0.0f);			// Top
				gl.glVertex3f(-1.0f,-1.0f, 0.0f);			// Bottom Left
				gl.glVertex3f( 1.0f,-1.0f, 0.0f);			// Bottom Right
			gl.glEnd();										// Finished Drawing The Triangle
			
		}
	}
}