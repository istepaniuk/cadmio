using GtkGL;
using gl = Tao.OpenGl.Gl;

namespace Cadmio
{
	
	public class Axis : BaseEntity, GtkGL.IGLObject
	{
		public Axis (int ID, Vector3 Vector, Vector3 Position) : base (ID)
		{
			Name = "Axis " + ID.ToString();
		  	Lighting = false;
			this.position = Position;
			this.vector = Vector;
			vector.Normalize();
			double size = 8;
			p1 = new Vector3((position + vector)*size);
			p2 = new Vector3((position - vector)*size);
		}
		
		private Vector3 vector, p1, p2;
		
		public Vector3 Vector 
		{ 
			get { return vector ; }  
			set 
			{
				vector = value;
				if (vector.Magnitude > 0)
					vector.Normalize();
			}
		}
		
		private Vector3 position = new Vector3(0,0,0);
		public Vector3 Position 
		{
			get { return position; }
		}
		
	    protected override void DrawObject ()
        {
			gl.glLineWidth(1.5f);
			gl.glBegin(gl.GL_LINES);
    		gl.glColor4f(0.7f, 0, 0, 0.98f);
    		gl.glVertex3d(p1.X, p1.Y, p1.Z);
    		gl.glVertex3d(p2.X, p2.Y, p2.Z);
			gl.glEnd();
		}
	}
}