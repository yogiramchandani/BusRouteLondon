TFL = {
    init: function() {
        var busStops = new TFL.BusStops();
        var view = new TFL.MapSectionView({ el: "#map_canvas", collection: busStops });
    }
};

    TFL.BusStops = Backbone.Collection.extend({
        baseurl: '/api/BusStop/',
        initialize: function (models) {
            var self = this;
            this._meta = {};
            return self;
        },
        search: function(lat, lng, radius) {
            this.url = this.baseurl + lat + '/' + lng + '/' + radius;
        },
        parse: function (response) {
            this._meta["totalCount"] = response.TotalCount;
            return response.Stops;
        }
    });

    TFL.MapSectionView = Backbone.View.extend({
        
        initialize: function () {
            this.render();
        },

        render: function () {
            var mapOptions = {
                center: new google.maps.LatLng(51.515, -0.141),
                maxZoom: 18,
                minZoom: 13,
                zoom: 14,
                scaleControl: true,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            };
            // 
            var map = new google.maps.Map(this.el, mapOptions);
            var self = this;
            var busStopInfoWindows = [];

            var busStopMarkers = {};
            _.extend(busStopMarkers, Backbone.Events);
            
            busStopMarkers.on("click", function(infoWindow, marker) {
                google.maps.event.addListener(map, 'click', function () { infoWindow.close(); });
                google.maps.event.addListener(marker, 'click', function (ev) {
                    _.each(busStopInfoWindows, function (info) {
                        info.close();
                    });
                    infoWindow.setPosition(ev.latLng);
                    infoWindow.open(map);
                });
            });
            
            var displayMarkers = function() { self.tilesLoaded(map, busStopInfoWindows, busStopMarkers); };
            google.maps.event.addListener(map, 'dragend', displayMarkers);
            google.maps.event.addListener(map, 'tilesloaded', displayMarkers);
            return this;
        },
        
        tilesLoaded: function (map, busStopInfoWindows, busStopMarkers) {
            var self = this;
            var bounds = map.getBounds();
            var center = bounds.getCenter();
            var ne = bounds.getNorthEast();

            var radiusMeter = google.maps.geometry.spherical.computeDistanceBetween(center, ne);
            var radiusMiles = radiusMeter * 0.000621371192;
            this.collection.search(center.lat(), center.lng(), radiusMiles);
            this.collection.fetch();
            _.each(this.collection.models, function (stop) {
                if(!busStopMarkers[stop.get("Id")])
                {
                    var marker = self.createMarkerOptions(map, stop);
                    
                    var infoWindow = new google.maps.InfoWindow({ content: stop.get("BusStopName") + "<br />" + "Bus stop code: " + stop.get("BusStopCode") });
                    
                    busStopMarkers[stop.get("Id")] = marker;
                    busStopMarkers.trigger("click", infoWindow, marker);
                    
                    busStopInfoWindows.push(infoWindow);
                }
            }, this);
        },
        createMarkerOptions: function(map, stop) {
            var stopCircleOptions = {
                strokeColor: "#FF0000",
                strokeOpacity: 0.8,
                strokeWeight: 1,
                fillColor: "#FF0000",
                fillOpacity: 0.35,
                map: map,
                center: new google.maps.LatLng(stop.get("Latitude"), stop.get("Longitude")),
                radius: 8
            };
            var busStopMarker = new google.maps.Circle(stopCircleOptions);
            return busStopMarker;
        }
    });
    
    function initialize() {
        TFL.init();
    }
    
    function loadScript() {
        var script = document.createElement("script");
        script.type = "text/javascript";
        script.src = "http://maps.googleapis.com/maps/api/js?key=AIzaSyDelN_I3DeYdJrMUp43bMxOOpgcDwCjy9o&sensor=false&callback=initialize&libraries=geometry";
        document.body.appendChild(script);
    }

    window.onload = loadScript;