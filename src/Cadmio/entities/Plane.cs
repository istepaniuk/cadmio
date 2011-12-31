using System;
using GtkGL;
using gl = Tao.OpenGl.Gl;

namespace Cadmio
{
	
	public class Plane: BaseEntity, IGLObject
	{
		public Plane (int ID, Vector3 Normal) : base(ID)
		{
			Name = "Plane " + ID.ToString();
		  	Lighting = false;
			this.normal = Normal;
			Init();
		}
		private Vector3 normal;
		public Vector3 Normal 
		{ 
			get { return normal ; }  
		}

     	private double size = 5.5;
		public double Size 
		{
		  get { return size; }			
		}
		
		private double [] position = new double[] {0,0,0};
		public double [] Position 
		{
			get { return position; }
		}

        protected override void DrawObject ()
        {
         	gl.glMatrixMode(gl.GL_MODELVIEW);
			gl.glPushMatrix();						
			
			Vector3 normalProyection = new Vector3(Normal);
			normalProyection.Z = 0;
			if (normalProyection.Magnitude > 0 )
				normalProyection.Normalize();
			
			Normal.Normalize();
			double angle = 360 * Vector3.Angle(Normal, Vector3.zAxis) / (Math.PI * 2);
			Vector3 axis = Vector3.CrossProduct(Normal, Vector3.zAxis); 
			
			gl.glRotated( angle,axis.X,axis.Y, axis.Z);			
			
			angle = 360 * Vector3.Angle(normalProyection, Vector3.xAxis) / (Math.PI * 2);
			
			gl.glRotated(angle,0,0,1);

			gl.glBegin(gl.GL_LINES);
    		gl.glColor4f (1f, 0f, 0f,0.9f);
			gl.glVertex3d(-size,  size, 0.001f);
			gl.glVertex3d( size,  size, 0.001f);
			gl.glVertex3d( size,  -size, 0.001f);
			gl.glVertex3d( size,   size, 0.001f);
			gl.glVertex3d( -size,  size, 0.001f);
			gl.glVertex3d( -size, -size, 0.001f);
    		gl.glVertex3d(-size, -size, 0.001f);
    		gl.glVertex3d( size, -size, 0.001f);
			gl.glEnd();
			
			gl.glBegin(gl.GL_QUADS);
			gl.glColor4f(1f, 1f, 0f,0.3f);
			gl.glVertex3d(-size,  size, 0);
			gl.glVertex3d( size,  size, 0);
			gl.glVertex3d( size,  -size, 0);
			gl.glVertex3d(-size,  -size, 0);
			gl.glEnd();
			
			gl.glPopMatrix();
		}
    }
}