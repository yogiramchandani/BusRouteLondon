/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
/*  The transforms below have been adapted from Chris Veness's most excellent JavaScript          */
/*  library:                                                                                      */
/*                                                                                                */
/*  www.movable-type.co.uk                                                                        */
/*  Coordinate transformations, lat/Long WGS-84 <=> OSGB36  (c) Chris Veness 2005-2012            */
/*   - www.movable-type.co.uk/scripts/coordtransform.js                                           */
/*   - www.movable-type.co.uk/scripts/latlong-convert-coords.html                                 */
/*   - www.movable-type.co.uk/scripts/gridref.js                                                  */
/*   - www.movable-type.co.uk/scripts/latlon-gridref.html                                         */
/* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -  */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BusRouteLondon.Web;

namespace BusRouteLondon.Migration
{
    public class OSGB36ToWGS84 : ISpatialCoordinateConverter
    {
        private ConcurrentDictionary<string, LatLong> Cached;
        
        public OSGB36ToWGS84()
        {
            Cached = new ConcurrentDictionary<string, LatLong>();
        }

        public void ConvertRoutes(List<BusRoute> routes)
        {
            Parallel.ForEach(routes, route =>
            {
                if (route.Stop.Longitude != 0 && route.Stop.Latitude != 0)
                {
                    return;
                }
                var result = Convert(route.Stop.Easting, route.Stop.Northing);

                route.Stop.Latitude = result.Lat;
                route.Stop.Longitude = result.Long;
            });
        }

        public LatLong Convert(int easting, int northing)
        {
            string key = string.Format("{0}:{1}", easting, northing);

            if (!Cached.ContainsKey(key))
            {
                Cached.TryAdd(key, ConvertOSGB36ToWGS84(northing, easting));
            }
            return Cached[key];
        }
        
        public enum EllipseEnum
        {
            WGS84,
            GRS80,
            Airy1830,
            AiryModified,
            Intl1924
        }
        
        // ellipse parameters
        private static readonly ReadOnlyDictionary<EllipseEnum, EllipseData> Ellipse = 
            new ReadOnlyDictionary<EllipseEnum, EllipseData>(new Dictionary<EllipseEnum, EllipseData>
            {
                {EllipseEnum.WGS84, new EllipseData(6378137, 6356752.3142,1/298.257223563)},
                {EllipseEnum.Airy1830, new EllipseData(6377563.396, 6356256.910,1/299.3249646)}
            });

        // ED50: og.decc.gov.uk/en/olgs/cms/pons_and_cop/pons/pon4/pon4.aspx
        // strictly, Ireland 1975 is from ETRF89: qv 
        // www.osi.ie/OSI/media/OSI/Content/Publications/transformations_booklet.pdf
        // www.ordnancesurvey.co.uk/oswebsite/gps/information/coordinatesystemsinfo/guidecontents/guide6.html#6.5
        // helmert transform parameters from OSGB36 to WGS84
        private static readonly TransformData TransformFromOSGB36ToWGS84 = new TransformData(446.448, -125.157, 542.060, 0.1502, 0.2470, 0.8421, -20.4894);

        /**
         * Convert lat/lon point in WGS84 to OSGB36
         *
         * @param  {LatLon} pWGS84: lat/lon in WGS84 reference frame
         * @return {LatLon} lat/lon point in OSGB36 reference frame
         */
        private LatLong ConvertOSGB36ToWGS84(double northing, double easting)
        {
            LatLong LatLongOSGB36 = OSGB36GridToLatLong(northing, easting);
            return ConvertEllipsoidOSGB36ToWGS84(LatLongOSGB36);
        }

        private LatLong OSGB36GridToLatLong(double northing, double easting)
        {
            const double a = 6377563.396;
            const double b = 6356256.910; // Airy 1830 major & minor semi-axes
            const double f0 = 0.9996012717; // NatGrid scale factor on central meridian
            const double lat0 = 49 * Math.PI / 180;
            const double lon0 = -2 * Math.PI / 180; // NatGrid true origin
            const int n0 = -100000;
            const int e0 = 400000; // northing & easting of true origin, metres
            const double e2 = 1 - (b * b) / (a * a); // eccentricity squared
            const double n = (a - b) / (a + b);
            const double n2 = n * n;
            const double n3 = n * n * n;

            var lat = lat0;
            var m = 0d;
            do
            {
                lat = (northing - n0 - m) / (a * f0) + lat;

                var ma = (1 + n + (5 / 4) * n2 + (5 / 4) * n3) * (lat - lat0);
                var mb = (3 * n + 3 * n * n + (21 / 8) * n3) * Math.Sin(lat - lat0) * Math.Cos(lat + lat0);
                var mc = ((15 / 8) * n2 + (15 / 8) * n3) * Math.Sin(2 * (lat - lat0)) * Math.Cos(2 * (lat + lat0));
                var md = (35 / 24) * n3 * Math.Sin(3 * (lat - lat0)) * Math.Cos(3 * (lat + lat0));
                m = b * f0 * (ma - mb + mc - md); // meridional arc

            } while (northing - n0 - m >= 0.00001); // i.e. until < 0.01mm

            var cosLat = Math.Cos(lat);
            var sinLat = Math.Sin(lat);
            var nu = a * f0 / Math.Sqrt(1 - e2 * sinLat * sinLat); // transverse radius of curvature
            var rho = a * f0 * (1 - e2) / Math.Pow(1 - e2 * sinLat * sinLat, 1.5); // meridional radius of curvature
            var eta2 = nu / rho - 1;

            var tanLat = Math.Tan(lat);
            var tan2Lat = tanLat * tanLat;
            var tan4Lat = tan2Lat * tan2Lat;
            var tan6Lat = tan4Lat * tan2Lat;
            var secLat = 1 / cosLat;
            var nu3 = nu * nu * nu;
            var nu5 = nu3 * nu * nu;
            var nu7 = nu5 * nu * nu;
            var vii = tanLat / (2 * rho * nu);
            var viii = tanLat / (24 * rho * nu3) * (5 + 3 * tan2Lat + eta2 - 9 * tan2Lat * eta2);
            var ix = tanLat / (720 * rho * nu5) * (61 + 90 * tan2Lat + 45 * tan4Lat);
            var x = secLat / nu;
            var xi = secLat / (6 * nu3) * (nu / rho + 2 * tan2Lat);
            var xii = secLat / (120 * nu5) * (5 + 28 * tan2Lat + 24 * tan4Lat);
            var xiia = secLat / (5040 * nu7) * (61 + 662 * tan2Lat + 1320 * tan4Lat + 720 * tan6Lat);

            var dE = (easting - e0);
            var dE2 = dE * dE;
            var dE3 = dE2 * dE;
            var dE4 = dE2 * dE2;
            var dE5 = dE3 * dE2;
            var dE6 = dE4 * dE2;
            var dE7 = dE5 * dE2;
            lat = lat - vii * dE2 + viii * dE4 - ix * dE6;
            var lon = lon0 + x * dE - xi * dE3 + xii * dE5 - xiia * dE7;

            return new LatLong(ToDegree(lat), ToDegree(lon));
        }

        /**
         * Convert lat/lon from one ellipsoidal model to another
         *
         * q.v. Ordnance Survey 'A guide to coordinate systems in Great Britain' Section 6
         *      www.ordnancesurvey.co.uk/oswebsite/gps/docs/A_Guide_to_Coordinate_Systems_in_Great_Britain.pdf
         *
         * @private
         * @param {LatLon}   point: lat/lon in source reference frame
         * @param {Number[]} sourceEllipse:    source ellipse parameters
         * @param {Number[]} t:     Helmert transform parameters
         * @param {Number[]} targetEllipse:    target ellipse parameters
         * @return {Coord} lat/lon in target reference frame
         */
        private LatLong ConvertEllipsoidOSGB36ToWGS84(LatLong point)
        {
            var t = TransformFromOSGB36ToWGS84;
            var sourceEllipse = Ellipse[EllipseEnum.Airy1830];
            var targetEllipse = Ellipse[EllipseEnum.WGS84];
            // -- 1: convert polar to Cartesian coordinates (using ellipse 1)

            var lat = ToRadian(point.Lat);
            var lon = ToRadian(point.Long);

            var a = sourceEllipse.A;
            var b = sourceEllipse.B;

            var sinPhi = Math.Sin(lat);
            var cosPhi = Math.Cos(lat);
            var sinLambda = Math.Sin(lon);
            var cosLambda = Math.Cos(lon);
            var h = 24.7; // for the moment

            var eSq = (a * a - b * b) / (a * a);
            var nu = a / Math.Sqrt(1 - eSq * sinPhi * sinPhi);

            var x1 = (nu + h) * cosPhi * cosLambda;
            var y1 = (nu + h) * cosPhi * sinLambda;
            var z1 = ((1 - eSq) * nu + h) * sinPhi;


            // -- 2: apply Helmert transform using appropriate params

            var tx = t.Tx;
            var ty = t.Ty;
            var tz = t.Tz;
            var rx = ToRadian(t.Rx / 3600); // normalise seconds to radians
            var ry = ToRadian(t.Ry / 3600);
            var rz = ToRadian(t.Rz / 3600);
            var s1 = t.S / 1e6 + 1; // normalise ppm to (s+1)

            // apply transform
            var x2 = tx + x1 * s1 - y1 * rz + z1 * ry;
            var y2 = ty + x1 * rz + y1 * s1 - z1 * rx;
            var z2 = tz - x1 * ry + y1 * rx + z1 * s1;


            // -- 3: convert Cartesian to polar coordinates (using ellipse 2)

            a = targetEllipse.A;
            b = targetEllipse.B;
            var precision = 4 / a; // results accurate to around 4 metres

            eSq = (a * a - b * b) / (a * a);
            var p = Math.Sqrt(x2 * x2 + y2 * y2);
            var phi = Math.Atan2(z2, p * (1 - eSq));
            var phiP = 2 * Math.PI;
            while (Math.Abs(phi - phiP) > precision)
            {
                nu = a / Math.Sqrt(1 - eSq * Math.Sin(phi) * Math.Sin(phi));
                phiP = phi;
                phi = Math.Atan2(z2 + eSq * nu * Math.Sin(phi), p);
            }
            var lambda = Math.Atan2(y2, x2);
            h = p / Math.Cos(phi) - nu;

            return new LatLong(ToDegree(phi), ToDegree(lambda), h);
        }

        public struct TransformData
        {
            public TransformData(double tx, double ty, double tz, double rx, double ry, double rz, double s)
                : this()
            {
                Tx = tx;
                Ty = ty;
                Tz = tz;
                Rx = rx;
                Ry = ry;
                Rz = rz;
                S = s;
            }

            public double Tx { get; private set; }
            public double Ty { get; private set; }
            public double Tz { get; private set; }

            public double Rx { get; private set; }
            public double Ry { get; private set; }
            public double Rz { get; private set; }

            public double S { get; private set; }
        }

        public struct EllipseData
        {
            public EllipseData(double a, double b, double c)
                : this()
            {
                A = a;
                B = b;
                C = c;
            }

            public double A { get; private set; }
            public double B { get; private set; }
            public double C { get; private set; }
        }
        
        public double ToDegree(double radian)
        {
            return (radian * 180) / Math.PI;
        }

        public double ToRadian(double degree)
        {
            return (degree * Math.PI) / 180;
        }
    }

    public interface ISpatialCoordinateConverter
    {
        LatLong Convert(int easting, int northing);
    }

    public struct LatLong
    {
        public LatLong(double lat, double @long, double rad = 6371)
            : this()
        {
            Lat = lat;
            Long = @long;
            Rad = rad;
        }

        public double Lat { get; private set; }
        public double Long { get; private set; }
        public double Rad { get; private set; }
    }
}