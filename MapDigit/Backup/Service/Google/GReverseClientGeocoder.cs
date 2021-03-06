﻿//------------------------------------------------------------------------------
//                         COPYRIGHT 2009 GUIDEBEE
//                           ALL RIGHTS RESERVED.
//                     GUIDEBEE CONFIDENTIAL PROPRIETARY 
///////////////////////////////////// REVISIONS ////////////////////////////////
// Date       Name                 Tracking #         Description
// ---------  -------------------  ----------         --------------------------
// 20JUN2009  James Shen                 	          Initial Creation
////////////////////////////////////////////////////////////////////////////////
//--------------------------------- IMPORTS ------------------------------------

//--------------------------------- PACKAGE -----------------------------------
using System;
using System.Collections;
using System.Net;
using MapDigit.AJAX;
using MapDigit.GIS.Geometry;
using MapDigit.GIS.Raster;

namespace MapDigit.GIS.Service.Google
{
    //[-------------------------- MAIN CLASS ----------------------------------]
    ////////////////////////////////////////////////////////////////////////////
    //----------------------------- REVISIONS ----------------------------------
    // Date       Name                 Tracking #         Description
    // --------   -------------------  -------------      ----------------------
    // 20JUN2009  James Shen                 	          Initial Creation
    ////////////////////////////////////////////////////////////////////////////
    /**
     * This class is used to communicate directly with Google servers to obtain
     * geocodes for user specified addresses. In addition, a geocoder maintains
     * its own cache of addresses, which allows repeated queries to be answered
     * without a Round trip to the server.
     * <hr><b>&copy; Copyright 2009 Guidebee, Inc. All Rights Reserved.</b>
     * @version     1.00, 20/06/09
     * @author      Guidebee, Inc.
     */
    public sealed class GReverseClientGeocoder : IReverseGeocoder
    {

        ////////////////////////////////////////////////////////////////////////////
        //--------------------------------- REVISIONS ------------------------------
        // Date       Name                 Tracking #         Description
        // ---------  -------------------  -------------      ----------------------
        // 20JUN2009  James Shen                 	          Initial Creation
        ////////////////////////////////////////////////////////////////////////////
        /**
         * Default constructor.
         */
        public GReverseClientGeocoder()
        {
            _reverseAddressQuery = new ReverseAddressQuery();
        }

        ////////////////////////////////////////////////////////////////////////////
        //--------------------------------- REVISIONS ------------------------------
        // Date       Name                 Tracking #         Description
        // ---------  -------------------  -------------      ----------------------
        // 20JUN2009  James Shen                 	          Initial Creation
        ////////////////////////////////////////////////////////////////////////////
        /**
         * Set google china or not.
         * @param china query china or not.
         */
        public void SetChina(bool china)
        {
            _isChina = china;
        }

        ////////////////////////////////////////////////////////////////////////////
        //--------------------------------- REVISIONS ------------------------------
        // Date       Name                 Tracking #         Description
        // ---------  -------------------  -------------      ----------------------
        // 20JUN2009  James Shen                 	          Initial Creation
        ////////////////////////////////////////////////////////////////////////////
        /**
         * Set google query key.
         * @param key google query key.
         */
        public void SetGoogleKey(string key)
        {
            _queryKey = key;
        }

        ////////////////////////////////////////////////////////////////////////////
        //--------------------------------- REVISIONS ------------------------------
        // Date       Name                 Tracking #         Description
        // ---------  -------------------  -------------      ----------------------
        // 20JUN2009  James Shen                 	          Initial Creation
        ////////////////////////////////////////////////////////////////////////////
        /**
         * Sends a request to Google servers to geocode the specified address
         * @param address  address to query
         * @param listener callback when query is done.
         */
        public void GetLocations(string address, IReverseGeocodingListener listener)
        {
            _listener = listener;
            _searchAddress = address;
            MapPoint mapPoint = (MapPoint)_addressCache[address];

            if (mapPoint == null)
            {
                if (!_isChina)
                {
                    Arg[] args = {
                    new Arg("ll", address),
                    new Arg("output", "js"),
                    new Arg("oe", "utf8"),
                    new Arg("key", _queryKey),
                    null
                };
                    Request.Get(SEARCH_BASE, args, null, _reverseAddressQuery, this);
                }
                else
                {
                    Arg[] args = {
                    new Arg("ll", address),
                    new Arg("output", "js"),
                    new Arg("oe", "utf8"),
                    null
                };
                    Request.Get(SEARCH_BASE_CHINA, args, null, _reverseAddressQuery, this);
                }
            }
            else
            {
                MapPoint[] mapPoints = new MapPoint[1];
                mapPoints[0] = mapPoint;
                listener.Done(mapPoint.Name, mapPoints);
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        //--------------------------------- REVISIONS ------------------------------
        // Date       Name                 Tracking #         Description
        // ---------  -------------------  -------------      ----------------------
        // 20JUN2009  James Shen                 	          Initial Creation
        ////////////////////////////////////////////////////////////////////////////
        /**
         * @inheritDoc
         */
        public void GetLocations(int mapType, string address, IReverseGeocodingListener listener)
        {
            _isChina = mapType == MapType.MICROSOFTCHINA ||
                        mapType == MapType.GOOGLECHINA || mapType == MapType.MAPABCCHINA;
            SetChina(_isChina);
            SetGoogleKey(GoogleQueryKeys[GoogleKeyIndex]);
            GetLocations(address,
                    listener);
            GoogleKeyIndex++;
            GoogleKeyIndex %= 10;
        }

        private const string SEARCH_BASE = "http://maps.google.com/maps/geo";
        private const string SEARCH_BASE_CHINA = "http://ditu.google.cn/maps/geo";
        readonly Hashtable _addressCache = new Hashtable();
        string _searchAddress;
        IReverseGeocodingListener _listener;
        readonly ReverseAddressQuery _reverseAddressQuery;
        private string _queryKey = "ABQIAAAAi44TY0V29QjeejKd2l3ipRTRERdeAiwZ9EeJWta3L_JZVS0bOBQlextEji5FPvXs8mXtMbELsAFL0w";
        private bool _isChina;
        private static readonly string[] GoogleQueryKeys = {
        "ABQIAAAAHxBdP31K2IukU7-aAA8n5hRcdv_D3MipQG6beFVt4q3n2KstuxQVPGsK1seABGQPugXw_P7Iua0JYw",
        "ABQIAAAAHxBdP31K2IukU7-aAA8n5hRr3VMBZ1cGe19qTgaCju5hrS8dIxSVwolc1mXM0pUIqSvJSNaW7jJUiA",
        "ABQIAAAAHxBdP31K2IukU7-aAA8n5hRm_ifamDDETX3GYECVeBf43IL7kxQhowIvbl9G-Mq1Jo874g3vZbr9KA",
        "ABQIAAAAHxBdP31K2IukU7-aAA8n5hTdoKf24hPXAkfeSPvoX63LdjNnwhTXeivbZPtE5W6vLnal3MgqR1Q4og",
        "ABQIAAAAHxBdP31K2IukU7-aAA8n5hQHwqwnNik4w_uH95OtQPrGD8h2aRQkX34t6brsYYQjMh5Al7WxZC-uRQ",
        "ABQIAAAAHxBdP31K2IukU7-aAA8n5hTxDsZgO1TyNw5Fb7lqwb1yrhjwjBRA87P_DQ_K07IWadLOQuyPYDfHIA",
        "ABQIAAAAHxBdP31K2IukU7-aAA8n5hQma3cdF9cz-FT2e3x_QfYqxZ-lIBQLKb6_-IocP_EZaz6BpXiLhuD8fg",
        "ABQIAAAAHxBdP31K2IukU7-aAA8n5hRm5GNFjZ8GN__mSLFDVmdMUufGqxTxofYdQZGsDgJOJ6_h-Q7HO4WF8w",
        "ABQIAAAAHxBdP31K2IukU7-aAA8n5hTEKgbPjtpwuJgXSRRhfbfuAHQlfRRdwtWTdkWiS7_AQmBiH4zhIHsUTQ",
        "ABQIAAAAi44TY0V29QjeejKd2l3ipRTRERdeAiwZ9EeJWta3L_JZVS0bOBQlextEji5FPvXs8mXtMbELsAFL0w"
    };
        private static int GoogleKeyIndex;

        internal class ReverseAddressQuery : IRequestListener
        {
            private static void SearchResponse(GReverseClientGeocoder reverseGeoCoder, Response response)
            {
                MapPoint[] mapPoints = null;
                Exception ex = response.GetException();
                if (ex != null || response.GetCode() != HttpStatusCode.OK)
                {

                    if (reverseGeoCoder._listener != null)
                    {
                        reverseGeoCoder._listener.Done(reverseGeoCoder._searchAddress, null);
                    }
                    return;
                }
                try
                {
                    Result result = response.GetResult();
                    result.GetAsString("name");
                    int resultCount = result.GetSizeOfArray("Placemark");
                    if (resultCount > 0)
                    {
                        mapPoints = new MapPoint[resultCount];
                        for (int i = 0; i < resultCount; i++)
                        {
                            mapPoints[i] = new MapPoint();
                            mapPoints[i].Name = result.GetAsString("Placemark[" + i + "].address");
                            string location = result.GetAsString("Placemark[" + i + "].Point.coordinates");
                            GeoLatLng latLng = MapLayer.FromStringToLatLng(location);
                            mapPoints[i].SetPoint(latLng);

                        }
                        if (reverseGeoCoder._addressCache.Count > 24)
                        {
                            for (int j = 0; j < 12; j++)
                            {
                                ICollection keys = reverseGeoCoder._addressCache.Keys;
                                foreach (var o in keys)
                                {
                                    reverseGeoCoder._addressCache.Remove(o);
                                    break;
                                }

                            }
                        }
                        reverseGeoCoder._addressCache.Add(mapPoints[0].Name, mapPoints[0]);
                    }

                }
                catch (Exception)
                {


                }
                if (reverseGeoCoder._listener != null)
                {
                    reverseGeoCoder._listener.Done(reverseGeoCoder._searchAddress, mapPoints);
                }

            }

            public void ReadProgress(Object context, int bytes, int total)
            {
                if (context is GReverseClientGeocoder)
                {
                    GReverseClientGeocoder reverseGeoCoder = (GReverseClientGeocoder)context;
                    reverseGeoCoder._listener.ReadProgress(bytes, total);
                }
            }

            public void WriteProgress(Object context, int bytes, int total)
            {
            }

            public void Done(Object context, Response response)
            {
                if (context is GReverseClientGeocoder)
                {
                    GReverseClientGeocoder reverseGeoCoder = (GReverseClientGeocoder)context;
                    SearchResponse(reverseGeoCoder, response);
                }
            }

            public void Done(Object context, string rawResult)
            {

            }
        }

    }


}
