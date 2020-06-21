using System;
using System.Collections.Generic;


namespace advtest
{ 
	public class SerialNumberGenerator
	{
		// Statisches Member bietet sich hier an!
		private static volatile SerialNumberGenerator instance;

		public static SerialNumberGenerator Instance
		{
			get
            {
				if( instance == null )
                {
					instance = new SerialNumberGenerator();

                }
				return instance;
            }
		}

		private int count; 

		// Oho, ein privater Konstruktur. 
		private SerialNumberGenerator()
        {

        }
		public virtual int NextSerial
        {
			get
            {
				return ( ++count);
            }
        }
	}
}
