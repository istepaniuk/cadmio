using GtkGL;
using gl = Tao.OpenGl.Gl;

namespace Cadmio
{
	
	public class Point: BaseEntity, IGLObject
	{
		public Point (int ID, Vector3 Position) : base(ID)
		{
			Name = "Point " + ID.ToString();
		  	Lighting = false;
			this.position = Position;
			Init();
		}
		
		private Vector3 position = new Vector3(0,0,0);
		
		public Vector3 Position
		{ 
			get { return position ; }  
		}

        protected override void DrawObject ()
        {
         	float size = 0.05f;
			
			gl.glPushMatrix();
			
			gl.glTranslated(Position.X,Position.Y,Position.Z);
			
			gl.glBegin(gl.GL_LINES);
		    gl.glLineWidth(1f);

			gl.glColor4f(0, 0, 0.7f,0.9f);
		    
			// x-axis
    		gl.glVertex3f(-size, 0, 0);
    		gl.glVertex3f( size, 0, 0);

			// y-axis
    		gl.glVertex3f(0,-size, 0);
    		gl.glVertex3f(0, size, 0);

		    // z-axis
    		gl.glVertex3f(0, 0, -size);
    		gl.glVertex3f(0, 0,  size);

    		gl.glEnd();
			
			gl.glPopMatrix();
		}
    }
}