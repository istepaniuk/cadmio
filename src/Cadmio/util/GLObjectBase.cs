using System;
using System.Collections;
using GtkGL;
using gl = Tao.OpenGl.Gl;

namespace GtkGL
{

	public abstract class GLObjectBase
	{
		protected ArrayList GLAreaList = null;

		protected int shapeID;

		public int glID {
			get { return shapeID; }
		}

		protected bool selected = false;

		public bool Selected {
			get { return selected; }
			set {
				if (value == true) {
					if (SelectedEvent != null)
						SelectedEvent (this, null);
				} else {
					if (DeSelectedEvent != null)
						DeSelectedEvent (this, null);
				}
				
				selected = value;
			}
		}

		protected float alphaValue = 1.0f;
		
		protected bool lighting = true;
		public bool Lighting {
			set { lighting = value; }
		}

		protected double[] scale = null;
		protected double[] translation = null;
		public GtkGL.Quaternion quat = null;

		// our Updated event handler
		public event EventHandler Updated;

		// our SelectedEvent event handler
		public event EventHandler SelectedEvent;

		// our DeSelectedEvent event handler
		public event EventHandler DeSelectedEvent;

		protected virtual void DrawObject ()
		{
		}

		// Cache the drawing of the object
		public void Init ()
		{
			// Do some genlist magic
			shapeID = gl.glGenLists (1);
			
			gl.glNewList (shapeID, gl.GL_COMPILE);
			DrawObject ();
			gl.glEndList ();
		}

		// Make setting of euler, quat and matrix an atomic action
		protected GtkGL.EulerRotation ERot {
			get { return quat.ToEulerRotation (); }
			set {
				if (value == null)
					quat = GtkGL.Quaternion.Identity;
				
				quat = value.ToQuaternion ();
			}
		}

		// Make setting of euler, quat and matrix an atomic action
		protected GtkGL.Quaternion Quat {
			get { return quat; }
			set {
				if (value == null)
					quat = GtkGL.Quaternion.Identity;
				
				quat = value;
			}
		}

		// Make setting of euler, quat and matrix an atomic action
		protected GtkGL.TransformationMatrix TransMatrix {
			get {
				//if (quat == null)
					Quat = GtkGL.Quaternion.Identity;
				
				if (translation == null) {
					translation = new double[3];
					translation[0] = 0;
					translation[1] = 0;
					translation[2] = 0;
				}
				
				if (scale == null) {
					scale = new double[3];
					scale[0] = 1;
					scale[1] = 1;
					scale[2] = 1;
				}
				
				// begin generating the transformation by converting the Quaternion into a rotation matrix
				GtkGL.TransformationMatrix tm = quat.ToTransMatrix ();
				
				// Next, add the translation X, Y and Z factors to the matrix
								/* 
				 *    | 1 0 0 X |
				 *    | 0 1 0 Y |
				 *    | 0 0 1 Z |
				 *    | 0 0 0 1 |
				 */
				tm.Matrix[12] = translation[0];
				tm.Matrix[13] = translation[1];
				tm.Matrix[14] = translation[2];
				
				// And finally, add the X, Y and Z scale factors to the matrix
								/*
        		 *    | X 0 0 0 |
        		 *    | 0 Y 0 0 |
        		 *    | 0 0 Z 0 |
        		 *    | 0 0 0 1 |
        		 */
				tm.Matrix[0] = scale[0];
				tm.Matrix[5] = scale[1];
				tm.Matrix[10] = scale[2];
				
				// Return the constructed TransformationMatrix
				return tm;
			}

			set {
				if (value == null)
					quat = GtkGL.Quaternion.Identity;
				
				// Set our internal quaternion from the Transformation Matrix
				quat = value.ToQuaternion ();
				
				if (translation == null)
					translation = new double[3];
				
				// Set our internal translation factors from the matrix
				translation[0] = value.Matrix[12];
				translation[1] = value.Matrix[13];
				translation[3] = value.Matrix[14];
				
				if (scale == null)
					scale = new double[3];
				
				// Set our internal scale factors from the matrix
				scale[0] = value.Matrix[0];
				scale[1] = value.Matrix[5];
				scale[2] = value.Matrix[10];
			}
		}

		public void Scale (double[] factor)
		{
			if (factor.Length != 3)
				return;
			
			this.Scale (factor[0], factor[1], factor[1]);
		}

		public void Scale (double factor)
		{
			this.Scale (factor, factor, factor);
		}

		public void ResetScale ()
		{
			scale[0] = 1;
			scale[1] = 1;
			scale[2] = 1;
		}
		public void Scale (double xFactor, double yFactor, double zFactor)
		{
			if (this.scale == null) {
				this.scale = new double[3];
				scale[0] = 1;
				scale[1] = 1;
				scale[2] = 1;
			}
			
			scale[0] *= xFactor;
			scale[1] *= yFactor;
			scale[2] *= zFactor;
			
			// Tell our handlers that we have been updated
			if (Updated != null)
				Updated (this, null);
		}

		public void Translate (float x, float y, float z)
		{
			this.Translate ((double)x, (double)y, (double)z);
		}

		public void Translate (double x, double y, double z)
		{
			if (this.translation == null)
				this.translation = new double[3];
			
			translation[0] = x;
			translation[1] = y;
			translation[2] = z;
			
			// Tell our handlers that we have been updated
			if (Updated != null)
				Updated (this, null);
		}

		public void Rotate (GtkGL.Quaternion q)
		{
			Quat *= q;
			
			Console.WriteLine("q: "+Quat.ToString());
			// Tell our handlers that we have been updated
			if (Updated != null)
				Updated (this, null);
		}

		public void Rotate (GtkGL.EulerRotation er)
		{
			Rotate (er.ToQuaternion ());
		}


		public void Rotate (GtkGL.TransformationMatrix tm)
		{
			Rotate (tm.ToQuaternion ());
		}

		public void ResetRotation ()
		{
			ResetRotation (true);
		}

		public void ResetRotation (bool doUpdate)
		{
			Quat = GtkGL.Quaternion.Identity;
			
			// Tell our handlers that we have been updated
			if (doUpdate && Updated != null)
				Updated (this, null);
		}

		public EulerRotation GetEulerRotation ()
		{
			return Quat.ToEulerRotation ();
		}

		public Quaternion GetQuaternion ()
		{
			if (quat != null)
				return quat;
			
			Quat = GtkGL.Quaternion.Identity;
			
			return Quat;
		}

		public TransformationMatrix GetTransformationMatrix ()
		{
			return TransMatrix;
		}

		public virtual bool Draw ()
		{
			gl.glPushMatrix ();
			
			gl.glMultMatrixd (this.TransMatrix.Matrix);
			
			if (!lighting)
				gl.glDisable(gl.GL_LIGHTING);
			else
    			gl.glEnable(gl.GL_LIGHTING);	
    		
			
			gl.glPushName (shapeID);
			// Draw the image from the display list
			
			//gl.glCallList (shapeID);
			
			DrawObject(); //not draw, callList!
			
			gl.glPopName ();
			
			gl.glPopMatrix ();
			
			return true;
		}
		
	}
}