TFL = {
    init: function() {
        var routes = new TFL.BusRoutes();
        var view = new TFL.MapSectionView({ el: "#map_canvas", collection: routes });
    }
};

    TFL.BusRoutes = Backbone.Collection.extend({
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
            google.maps.event.addListener(map, 'dragend', function () { self.tilesLoaded(map); });
            google.maps.event.addListener(map, 'tilesloaded', function () { self.tilesLoaded(map); });
            return this;
        },
        
        tilesLoaded : function(map){
            var bounds = map.getBounds();
            var center = bounds.getCenter();
            var ne = bounds.getNorthEast();

            var radiusMeter = google.maps.geometry.spherical.computeDistanceBetween(center, ne);
            var radiusMiles = radiusMeter * 0.000621371192;
            this.collection.search(center.lat(), center.lng(), radiusMiles);
            this.collection.fetch();
            _.each(this.collection.models, function(stop){
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
                busStops = new google.maps.Circle(stopCircleOptions);
            }, this);

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