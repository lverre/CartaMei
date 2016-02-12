namespace CartaMei.Mercator
{
    public class TransverseSphericalFormulae : SphericalFormulae
    {
        #region Properties

        public double ScaleFactor { get; set; }

        #endregion

        #region Overrides

        protected override double R
        {
            get { return base.R * this.ScaleFactor; }
        }
        
        #endregion
    }

    public class TransverseEllipsoidFormulae : EllipsoidFormulae
    {
        #region Properties

        public double ScaleFactor { get; set; }

        #endregion

        #region Overrides

        protected override double R
        {
            get { return base.R * this.ScaleFactor; }
        }

        #endregion
    }
}
