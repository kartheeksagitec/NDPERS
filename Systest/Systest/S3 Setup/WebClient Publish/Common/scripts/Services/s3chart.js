app.service('$Chart', ["$rootScope", function ($rootScope) {
    var p = ["#219ae7", "#294ec7", "#5E78CD", "#30437E", "#763555", "#421D2F", "#662A48", "#CD6E1F", "#e77b21", "#EE9F5D", "#f7caa5", "#e0ab7f", "#ef6000", "#1FA28E", "#25bba4", "#34E0C6", "#366657", "#6d2d80", "#6A5870", "#004C63", "#136B85", "#149398", "#107074", "#149398", "#107074"];
    var exampleNodes = [];
    var exampleLinks = [];
    // draws bar chart on giving input data, wrapper id;
    this.drawBarChart = function (dataset, wrapperid, propname, propvalue) {
        var chartWrapper = document.getElementById(wrapperid),
                 chartWrapperWidth = chartWrapper.clientWidth,
                 chartWrapperHeight = chartWrapper.clientHeight,
                 chartWrapperPaddingleft = parseFloat((window.getComputedStyle(chartWrapper, null).getPropertyValue('padding-left').replace("px", ""))),
                 chartWrapperPaddingRight = parseFloat((window.getComputedStyle(chartWrapper, null).getPropertyValue('padding-right').replace("px", ""))),
                 chartWrapperPaddingTop = parseFloat((window.getComputedStyle(chartWrapper, null).getPropertyValue('padding-top').replace("px", ""))),
                 chartWrapperPaddingBottom = parseFloat((window.getComputedStyle(chartWrapper, null).getPropertyValue('padding-bottom').replace("px", ""))),
                 totalWidth = chartWrapperWidth - chartWrapperPaddingleft - chartWrapperPaddingRight,
                 totalHeight = chartWrapperHeight - chartWrapperPaddingTop - chartWrapperPaddingBottom;

        var svg = d3.select(chartWrapper).append("svg"),
               margin = {
                   top: 20,
                   right: 20,
                   bottom: 35,
                   left: 40
               },
               width = totalWidth - margin.left - margin.right,
               height = totalHeight - margin.top - margin.bottom;

        svg.attr("width", totalWidth).attr("height", totalHeight);

        var xScale = d3.scale.ordinal()
            .domain(dataset.map(function (d) { return d[propname]; }))
            .rangeRoundBands([0, width], .1);

        var yScale = d3.scale.linear()
            .domain([0, d3.max(dataset, function (d) { return d[propvalue]; })])
            .range([height, 0]);

        var xAxis = d3.svg.axis()
            .scale(xScale)
            .orient("bottom")
            .innerTickSize(-height)
            .outerTickSize(0)
            .tickPadding(10);

        var yAxis = d3.svg.axis()
            .scale(yScale)
            .orient("left")
            .innerTickSize(-width)
            .outerTickSize(0)
            .tickPadding(10);

        var svg = d3.select("svg")
            .attr("width", width + margin.left + margin.right)
            .attr("height", height + margin.top + margin.bottom)
            .append("g")
            .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

        svg.append("g")
            .attr("class", "x axis")
            .attr("transform", "translate(0," + height + ")")
            .call(xAxis);

        svg.append("g")
            .attr("class", "y axis")
            .call(yAxis);

        // for bar columns
        svg.selectAll(".bar")
            .data(dataset)
            .enter().append("rect")
            .attr("class", "bar")
            .attr("x", function (d) {
                return xScale(d[propname]);
            })
            .attr("y", function (d) {
                return yScale(d[propvalue]);
            })
            .attr("width", xScale.rangeBand())
            .attr("height", function (d) {
                return height - yScale(d[propvalue]);
            })
            .attr("fill", "#dab45f");

        svg.selectAll(".x .tick text")
        .attr("transform", "translate(0,2)rotate(-12)");

    };
    this.drawStackedBarChart = function (dataset, wrapperid, propname, propvalue) {
        if ($('#' + wrapperid)[0])
        {
            $('#' + wrapperid)[0].innerHTML = "";
            var types = ["Entity", "LogicalRule", "DecisionTable", "ExcelMatrix", "ExcelScenario", "ObjectScenario", "ParameterScenario", "Lookup", "Maintenance", "Wizard", "UserControl", "Tooltip", "Prototype", "FormLinkLookup", "FormLinkMaintenance", "FormLinkWizard", "Report", "InboundFile", "OutboundFile", "Correspondence", "PDFCorrespondence", "BPMN", "BPMTemplate", "WorkflowMap"];
            var typecount = [];
            var maindata = dataset;
            var chartWrapper = document.getElementById(wrapperid),
                chartWrapperWidth = chartWrapper.clientWidth,
                chartWrapperHeight = chartWrapper.clientHeight,
                chartWrapperPaddingleft = parseFloat((window.getComputedStyle(chartWrapper, null).getPropertyValue('padding-left').replace("px", ""))),
                chartWrapperPaddingRight = parseFloat((window.getComputedStyle(chartWrapper, null).getPropertyValue('padding-right').replace("px", ""))),
                chartWrapperPaddingTop = parseFloat((window.getComputedStyle(chartWrapper, null).getPropertyValue('padding-top').replace("px", ""))),
                chartWrapperPaddingBottom = parseFloat((window.getComputedStyle(chartWrapper, null).getPropertyValue('padding-bottom').replace("px", ""))),
                totalWidth = chartWrapperWidth - chartWrapperPaddingleft - chartWrapperPaddingRight,
                totalHeight = chartWrapperHeight - chartWrapperPaddingTop - chartWrapperPaddingBottom;
            var margin = {
                top: 20,
                right: 50,
                bottom: 35,
                left: 20
            },
                width = totalWidth - margin.left - margin.right,
                height = totalHeight - margin.top - margin.bottom;
            var barWidth = 35;
            var border = 0.75;
            var bordercolor = 'grey';

            var x = d3.scale.ordinal().rangeRoundBands([0, width], .1);
            var y = d3.scale.linear().range([height, 0]);
            var xAxis = d3.svg.axis().scale(x).orient("bottom").outerTickSize(0)
                .tickPadding(10);
            var yAxis = d3.svg.axis().scale(y).orient("left").innerTickSize(-width)
                .outerTickSize(0)
                .tickPadding(10);

            var tip = d3.tip()
                .attr('class', 'd3-tip')
                .offset([-10, 0])
                .html(function (d) {
                    return "<strong>" + d.z + " :</strong> <span>" + d.y + "</span>";
                });

            var svg = d3.select(chartWrapper).append("svg").attr("width", totalWidth).attr("height", totalHeight)
                .append("g").attr("transform", "translate(" + margin.right + "," + margin.top + ")").call(tip);

            // actual data is prepared here
            var layers = d3.layout.stack()(types.map(function (c) {
                return maindata.map(function (d) {
                    return {
                        x: d[propname],
                        y: d[propvalue][c],
                        z: c
                    };
                });
            }));

            x.domain(layers[0].map(function (d) {
                return d.x;
            }));
            y.domain([0, d3.max(layers[layers.length - 1], function (d) {
                return d.y0 + d.y;
            })]).nice();

            // draw x-axis
            svg.append("g")
                .attr("class", "axis axis--x")
                .attr("transform", "translate(0," + height + ")")
                .call(xAxis);
            // draw y-axis , add a text "count" beside it
            svg.append("g")
                .attr("class", "axis axis--y")
                .call(yAxis)
                .append("text")
                .attr("transform", "rotate(-90)")
                .attr("y", 6)
                .attr("dy", ".71em")
                .style("text-anchor", "end")
                .text("Count");
            // draw bars 
            var layer = svg.selectAll(".layer")
                .data(layers)
                .enter().append("g")
                .attr("class", "layer")
                .style("fill", function (d, i) {
                    return p[i];
                });

            layer.selectAll("rect")
                .data(function (d) {
                    return d;
                })

                .enter().append("rect")
                .attr("x", function (d, i) {
                    return x(d.x) + (x.rangeBand() - barWidth) / 2;
                })
                .attr("y", function (d) {
                    return y(d.y + d.y0);
                })
                .attr("height", function (d) {
                    return y(d.y0) - y(d.y + d.y0);
                })
                .attr("width", Math.min(x.rangeBand(), barWidth))
                //.style("filter", "url(#drop-shadow)")
                .on('mouseover', tip.show)
                .on('mouseout', tip.hide);
        }
    };
    this.drawPieChart = function (dataset, wrapperid, propname, propvalue) {
        if ($('#' + wrapperid)[0])
        {
            $('#' + wrapperid)[0].innerHTML = "";
            var total = d3.sum(dataset, function (d) { return d3.sum(d3.values(d)); });
            var chartWrapper = document.getElementById("TfsChart"), chartWrapperWidth = chartWrapper.clientWidth, chartWrapperHeight = chartWrapper.clientHeight,
                inner = 70;
            var width = chartWrapperWidth, height = chartWrapperHeight, r = Math.min(width, height) / 2;
            var color = d3.scale.ordinal().range(["#a79773", "#dab45f", "#ffdf96"]);
            var vis = d3.select("#TfsChart").append("svg:svg")
                .data([dataset])
                .attr("width", chartWrapperWidth)
                .attr("height", chartWrapperHeight)
                .append("svg:g")
                .attr("transform", "translate(" + (r * 1 + ((Math.max(width, height) - r) / 4)) + "," + r * 1 + ")");
            var textTop = vis.append("text")
                .attr("dy", ".35em")
                .style("text-anchor", "middle")
                .attr("class", "textTop")
                .text("TOTAL")
                .attr("y", -10),
                textBottom = vis.append("text")
                    .attr("dy", ".35em")
                    .style("text-anchor", "middle")
                    .attr("class", "textBottom")
                    .text(total)
                    .attr("y", 10);

            var arc = d3.svg.arc().innerRadius(inner + 5).outerRadius(r - 10);
            var arcOver = d3.svg.arc().innerRadius(inner + 10).outerRadius(r - 5);
            var pie = d3.layout.pie().value(function (d) { return d[propvalue]; });
            var arcs = vis.selectAll("g.slice")
                .data(pie)
                .enter()
                .append("svg:g")
                .attr("class", "slice")
                .on("mouseover", function (d) {
                    d3.select(this).select("path").transition()
                        .duration(200)
                        .attr("d", arcOver);
                    textTop.text(d3.select(this).datum().data.Name)
                        .attr("y", -10);
                    textBottom.text(d3.select(this).datum().data.Count)
                        .attr("y", 10);
                })
                .on("mouseout", function (d) {
                    d3.select(this).select("path").transition()
                        .duration(100)
                        .attr("d", arc);
                    textTop.text("TOTAL")
                        .attr("y", -10);
                    textBottom.text(total);
                });

            arcs.append("svg:path")
                .attr("fill", function (d, i) { return color(i); })
                .attr("d", arc);

        //var legend = d3.select("#TfsChart").append("svg")
        //              .attr("class", "legend")
        //              .attr("width", r)
        //              .attr("height", r * 2)
        //              .selectAll("g")
        //              .data(dataset)
        //              .enter().append("g")
        //              .attr("transform", function (d, i) { return "translate(0," + i * 20 + ")"; });

        //                legend.append("rect")
        //                    .attr("width", 18)
        //                    .attr("height", 18)
        //                    .style("fill", function (d, i) { return color(i); });

        //                legend.append("text")
        //                    .attr("x", 24)
        //                    .attr("y", 9)
        //                    .attr("dy", ".35em")
        //                    .text(function (d) { return d[propname]; });
        }
    };

    this.drawErDiagram = function (strFileName) {
      
        var svg, tooltip, biHiSankey, path, defs, colorScale, highlightColorScale, isTransitioning;

        var OPACITY = {
            NODE_DEFAULT: 0.9,
            NODE_FADED: 0.1,
            NODE_HIGHLIGHT: 0.8,
            LINK_DEFAULT: 0.6,
            LINK_FADED: 0.05,
            LINK_HIGHLIGHT: 0.9
        },
          TYPES = ["Entity", ],
          TYPE_COLORS = ["#77b3d4"],
          TYPE_HIGHLIGHT_COLORS = ["#66c2a5", "#fc8d62", "#8da0cb", "#e78ac3", "#a6d854", "#ffd92f", "#e5c494"],
          LINK_COLOR = "#FFFFFF",
          INFLOW_COLOR = "#2E86D1",
          OUTFLOW_COLOR = "#D63028",
          NODE_WIDTH = 160,
          COLLAPSER = {
              RADIUS: NODE_WIDTH / 2,
              SPACING: 2
          },
          OUTER_MARGIN = 50,
          MARGIN = {
              TOP: (OUTER_MARGIN),
              RIGHT: OUTER_MARGIN,
              BOTTOM: OUTER_MARGIN,
              LEFT: OUTER_MARGIN
          },
          TRANSITION_DURATION = 400,
          HEIGHT = 700 - MARGIN.TOP - MARGIN.BOTTOM,
          WIDTH = 1400 - MARGIN.LEFT - MARGIN.RIGHT,
          LAYOUT_INTERATIONS = 32,
          REFRESH_INTERVAL = 7000;
        var calulateHieght = 0;
        for (var i = 0; i < exampleNodes.length; i++) {
            if (exampleNodes[i].attributes.length > 0) {
                calulateHieght = calulateHieght + (exampleNodes[i].attributes.length * 25);
            }
            else {
                calulateHieght = calulateHieght + 25;
            }
        }
        if (calulateHieght > HEIGHT) {
            HEIGHT = calulateHieght;
        }
        var formatNumber = function (d) {
            var numberFormat = d3.format(",.0f"); // zero decimal places
            return "£" + numberFormat(d);
        },

        menu = contextMenu().items('Navigate to Entity');

        formatFlow = function (d) {
            var flowFormat = d3.format(",.0f"); // zero decimal places with sign
            return "£" + flowFormat(Math.abs(d)) + (d < 0 ? " CR" : " DR");
        },

        // Used when temporarily disabling user interractions to allow animations to complete
        disableUserInterractions = function (time) {
            isTransitioning = true;
            setTimeout(function () {
                isTransitioning = false;
            }, time);
        },

        hideTooltip = function () {
            return tooltip.transition()
              .duration(TRANSITION_DURATION)
              .style("opacity", 0);
        },

        showTooltip = function () {
            return tooltip
              .style("left", d3.event.pageX + "px")
              .style("top", d3.event.pageY + 15 + "px")
              .transition()
                .duration(TRANSITION_DURATION)
                .style("opacity", 1);
        };

        colorScale = d3.scale.ordinal().domain(TYPES).range(TYPE_COLORS),
        highlightColorScale = d3.scale.ordinal().domain(TYPES).range(TYPE_HIGHLIGHT_COLORS),

        svg = d3.select("#" + strFileName + " [wrapper-erdiagram]").append("svg")
                .attr("width", WIDTH + MARGIN.LEFT + MARGIN.RIGHT)
                .attr("height", HEIGHT + MARGIN.TOP + MARGIN.BOTTOM)
              .append("g")
                .attr("transform", "translate(" + MARGIN.LEFT + "," + MARGIN.TOP + ")");

        svg.append("g").attr("id", "links");
        svg.append("g").attr("id", "nodes");
        svg.append("g").attr("id", "collapsers");

        tooltip = d3.select("#" + strFileName + " [wrapper-erdiagram]").append("div").attr("id", "tooltip");

        tooltip.style("opacity", 0)
            .append("p")
              .attr("class", "value");

        biHiSankey = d3.biHiSankey();

        // Set the biHiSankey diagram properties
        biHiSankey
          .nodeWidth(NODE_WIDTH)
          .nodeSpacing(1)
          .linkSpacing(4)
          .arrowheadScaleFactor(0.5) // Specifies that 0.5 of the link's stroke WIDTH should be allowed for the marker at the end of the link.
          .size([WIDTH, HEIGHT]);

        path = biHiSankey.link().curvature(0.45);

        defs = svg.append("defs");

        defs.append("marker")
          .style("fill", LINK_COLOR)
          .attr("id", strFileName + "arrowHead")
          .attr("viewBox", "0 0 10 10")
          .attr("refX", "5")
          .attr("refY", "5")
          .attr("markerUnits", "strokeWidth")
          .attr("markerWidth", "3")
          .attr("markerHeight", "3")
          .attr("orient", "auto")
          .append("path")
            .attr("d", "M 0 0 L 1 0 L 6 5 L 1 10 L 0 10 z");

        defs.append("marker")
                  .style("fill", INFLOW_COLOR)
                  .attr("id", strFileName + "arrowHeadOneToOne")
                  .attr("viewBox", "0 0 10 10")
                  .attr("refX", "5")
                  .attr("refY", "5")
                  .attr("markerUnits", "strokeWidth")
                  .attr("markerWidth", "3")
                  .attr("markerHeight", "3")
                  .attr("orient", "auto")
                  .append("path")
                    .attr("d", "M 0 0 L 1 0 L 6 5 L 1 10 L 0 10 z");

        defs.append("marker")
                         .style("fill", OUTFLOW_COLOR)
                         .attr("id", strFileName + "arrowHeadOneToMany")
                         .attr("viewBox", "0 0 10 10")
                         .attr("refX", "5")
                         .attr("refY", "5")
                         .attr("markerUnits", "strokeWidth")
                         .attr("markerWidth", "3")
                         .attr("markerHeight", "3")
                         .attr("orient", "auto")
                         .append("path")
                           .attr("d", "M 0 0 L 1 0 L 6 5 L 1 10 L 0 10 z");

        defs.append("marker")
          .style("fill", OUTFLOW_COLOR)
          .attr("id", strFileName + "arrowHeadInflow")
          .attr("viewBox", "0 0 10 10")
          .attr("refX", "5")
          .attr("refY", "5")
          .attr("markerUnits", "strokeWidth")
          .attr("markerWidth", "3")
          .attr("markerHeight", "3")
          .attr("orient", "auto")
          .append("path")
            .attr("d", "M 0 0 L 1 0 L 6 5 L 1 10 L 0 10 z");

        defs.append("marker")
          .style("fill", INFLOW_COLOR)
          .attr("id", strFileName + "arrowHeadOutlow")
          .attr("viewBox", "0 0 10 10")
          .attr("refX", "5")
          .attr("refY", "5")
          .attr("markerUnits", "strokeWidth")
          .attr("markerWidth", "3")
          .attr("markerHeight", "3")
          .attr("orient", "auto")
          .append("path")
            .attr("d", "M 0 0 L 1 0 L 6 5 L 1 10 L 0 10 z");

        function contextMenu() {
            var height,
                width,
                margin = 0.1, // fraction of width
                items = [],
                rescale = false,
                style = {
                    'rect': {
                        'mouseout': {
                            'fill': 'rgb(244,244,244)',
                            'stroke': 'white',
                            'stroke-width': '1px'
                        },
                        'mouseover': {
                            'fill': 'rgb(200,200,200)'
                        }
                    },
                    'text': {
                        'fill': 'steelblue',
                        'font-size': '13'
                    }
                };

            function menu(x, y, nodedata) {
                d3.select("#" + strFileName + " .context-menu").remove();
                scaleItems();

                // Draw the menu
                d3.select("#" + strFileName + ' svg')
                    .append('g').attr('class', 'context-menu')
                    .selectAll('tmp')
                    .data(items).enter()
                    .append('g').attr('class', 'menu-entry')
                    .style({ 'cursor': 'pointer' })
                    .on('mouseover', function () {
                        disableUserInterractions(500);
                        d3.select(this).select('rect').style(style.rect.mouseover)
                    })
                    .on('mouseout', function () {
                        disableUserInterractions(500);
                        d3.select(this).select('rect').style(style.rect.mouseout)
                    })
                     .on('click', function () {
                         $.connection.hubMain.server.navigateToFile(nodedata.id, "").done(function (objfile) {
                             $rootScope.openFile(objfile, false);
                         });
                     });

                d3.selectAll("#" + strFileName + ' svg .menu-entry')
                    .append('rect')
                    .attr('x', x)
                    .attr('y', function (d, i) { return y + (i * height); })
                    .attr('width', width)
                    .attr('height', height)
                    .style(style.rect.mouseout);

                d3.selectAll("#" + strFileName + ' svg .menu-entry')
                    .append('text')
                    .text(function (d) { return d; })
                    .attr('x', x)
                    .attr('y', function (d, i) { return y + (i * height); })
                    .attr('dy', height - margin / 2)
                    .attr('dx', margin)
                    .style(style.text);

                // Other interactions
                d3.select('body')
                    .on('click', function () {
                        d3.select("[wrapper-erdiagram] .context-menu").remove();
                    });

            }

            menu.items = function (e) {
                if (!arguments.length) return items;
                for (i in arguments) items.push(arguments[i]);
                rescale = true;
                return menu;
            }

            // Automatically set width, height, and margin;
            function scaleItems() {
                if (rescale) {                    
                    d3.select("#" + strFileName + ' svg').selectAll('tmp')
                        .data(items).enter()
                        .append('text')
                        .text(function (d) { return d; })
                        .style(style.text)
                        .attr('x', -1000)
                        .attr('y', -1000)
                        .attr('class', 'tmp');
                    var z = d3.selectAll("#" + strFileName + ' .tmp')[0]
                              .map(function (x) { return x.getBBox(); });
                    width = d3.max(z.map(function (x) { return x.width; }));
                    margin = margin * width;
                    width = width + 2 * margin;
                    height = d3.max(z.map(function (x) { return x.height + margin / 2; }));

                    // cleanup
                    d3.selectAll("#" + strFileName + ' .tmp').remove();
                    rescale = false;
                }
            }

            return menu;
        }

        function update() {
            var link, linkEnter, node, nodeEnter, nodeEnterBox, collapser, collapserEnter;

            function dragmove(node) {
                node.x = Math.max(0, Math.min(WIDTH - node.width, d3.event.x));
                node.y = Math.max(0, Math.min(HEIGHT - node.height, d3.event.y));
                d3.select(this).attr("transform", "translate(" + node.x + "," + node.y + ")");
                biHiSankey.relayout();
                svg.selectAll(".node").selectAll("rect").attr("height", function (d) { return d.height; });
                link.attr("d", path);
            }

            function containChildren(node) {
                node.children.forEach(function (child) {
                    child.state = "contained";
                    child.parent = this;
                    child._parent = null;
                    containChildren(child);
                }, node);
            }

            function expand(node) {
                node.state = "expanded";
                node.children.forEach(function (child) {
                    child.state = "collapsed";
                    child._parent = this;
                    child.parent = null;
                    containChildren(child);
                }, node);
            }

            function collapse(node) {
                node.state = "collapsed";
                containChildren(node);
            }

            function restoreLinksAndNodes() {
                link
                  .style("stroke", function (d, i) {
                      return d.type == "onetoone" ? INFLOW_COLOR : OUTFLOW_COLOR;
                  })
                  .style("marker-end", function (d) {
                      return d.type == "onetoone" ? 'url(#' + strFileName + 'arrowHeadOneToOne)' : 'url(#' + strFileName + 'arrowHeadOneToMany)';
                  })
                  .transition()
                    .duration(TRANSITION_DURATION)
                    .style("opacity", OPACITY.LINK_DEFAULT);

                node
                  .selectAll("rect")
                    .style("fill", function (d) {
                        d.color = colorScale(d.type.replace(/ .*/, ""));
                        return d.color;
                    })
                     .style("stroke", "white")
                .style("stroke-WIDTH", "4")
                    .style("fill-opacity", OPACITY.NODE_DEFAULT);

                node.filter(function (n) { return n.state === "collapsed"; })
                  .transition()
                    .duration(TRANSITION_DURATION)
                    .style("opacity", OPACITY.NODE_DEFAULT);
            }

            function showHideChildren(node) {
                disableUserInterractions(2 * TRANSITION_DURATION);
                hideTooltip();
                if (node.state === "collapsed") { expand(node); }
                else { collapse(node); }

                biHiSankey.relayout();
                update();
                link.attr("d", path);
                restoreLinksAndNodes();
            }

            function highlightConnected(g) {
                link.filter(function (d) { return d.type === "onetomany"; })
                  .style("marker-end", function () { return 'url(#' + strFileName + 'arrowHeadInflow)'; })
                  .style("stroke", OUTFLOW_COLOR)
                  .style("opacity", OPACITY.LINK_DEFAULT);

                link.filter(function (d) { return d.type === "onetoone"; })
                  .style("marker-end", function () { return 'url(#' + strFileName + 'arrowHeadOutlow)'; })
                  .style("stroke", INFLOW_COLOR)
                  .style("opacity", OPACITY.LINK_DEFAULT);
            }

            function fadeUnconnected(g) {
                link.filter(function (d) { return d.source !== g && d.target !== g; })
                  .style("marker-end", function () {
                      return 'url(#' + strFileName + 'arrowHead)';
                  })
                  .transition()
                    .duration(TRANSITION_DURATION)
                    .style("opacity", OPACITY.LINK_FADED);

                node.filter(function (d) {
                    return (d.name === g.name) ? false : !biHiSankey.connected(d, g);
                }).transition()
                  .duration(TRANSITION_DURATION)
                  .style("opacity", OPACITY.NODE_FADED);
            }

            node = svg.select("#nodes").selectAll(".node")
                .data(biHiSankey.collapsedNodes(), function (d) { return d.id; });

            node.transition()
              .duration(TRANSITION_DURATION)
              .attr("transform", function (d) { return "translate(" + d.x + "," + d.y + ")"; })
              .style("opacity", OPACITY.NODE_DEFAULT)
              .select("rect")
                .style("fill", function (d) {
                    d.color = colorScale(d.type.replace(/ .*/, ""));
                    return d.color;
                })
                .style("stroke", function (d) { return d3.rgb(colorScale(d.type.replace(/ .*/, ""))).darker(0.1); })
                .style("stroke-WIDTH", "1px")
                .attr("height", function (d) { return d.height; })
                .attr("width", biHiSankey.nodeWidth());

            node.exit()
              .transition()
                .duration(TRANSITION_DURATION)
                .attr("transform", function (d) {
                    var collapsedAncestor, endX, endY;
                    collapsedAncestor = d.ancestors.filter(function (a) {
                        return a.state === "collapsed";
                    })[0];
                    endX = collapsedAncestor ? collapsedAncestor.x : d.x;
                    endY = collapsedAncestor ? collapsedAncestor.y : d.y;
                    return "translate(" + endX + "," + endY + ")";
                })
                .remove();

            nodeEnter = node.enter().append("g").attr("class", "node");

            nodeEnter
              .attr("transform", function (d) {
                  var startX = d._parent ? d._parent.x : d.x,
                      startY = d._parent ? d._parent.y : d.y;
                  return "translate(" + startX + "," + startY + ")";
              })
              .style("opacity", 1e-6)
              .transition()
                .duration(TRANSITION_DURATION)
                .style("opacity", OPACITY.NODE_DEFAULT)
                .attr("transform", function (d) { return "translate(" + d.x + "," + d.y + ")"; });

            nodeEnter.append("text").attr("class", "node-name");

            nodeEnter.append("rect");

            nodeEnter.selectAll(".attributes")
                  .data(function (d) {
                      return d.attributes;
                  })
                  .enter().append("text")
                  .attr("x", 6)
                  .attr("y", function (d, i) { return i * 20 + 15 })
                  .attr("dy", ".35em")
                 .text(function (d, i) { return d })
                  .attr("class", "attributes");


            nodeEnter.select("rect")
            .style("fill", function (d) {
                d.color = colorScale(d.type.replace(/ .*/, ""));
                return d.color;
            })
              .style("stroke", "white")
              .style("stroke-WIDTH", "4")
              .attr("stroke-linecap", "round")
              .attr("height", function (d) { return d.height; })
              .attr("width", function (d) {
                  var rectMaxWidth = CalculateRectangleWidth(d);
                  if (rectMaxWidth > 160) {
                      rectMaxWidth = rectMaxWidth + 15;
                      d.width = rectMaxWidth;
                  }
                  return rectMaxWidth;
                  //if (this.parentNode && this.parentNode.getBBox().width>0) {
                  //    return this.parentNode.getBBox().width + 20;
                  //}
                  //else {
                  //    return biHiSankey.nodeWidth();
                  //}
              })
                .on('contextmenu', function (g) {
                    d3.event.preventDefault();
                    disableUserInterractions(500);
                    //console.log("clientX: " + d3.event.clientX + " - clientY: " + d3.event.clientY);
                    //console.log("pageX: " + d3.event.pageX + " - pageY: " + d3.event.pageY);
                    //console.log("offesetX: " + d3.event.offsetX + " - offesetY: " + d3.event.offsetY);
                    //console.log("mousex: " + d3.mouse(this)[0] + " - mouseY: " + d3.mouse(this)[1]);
                    //console.log("screenX: " + d3.event.screenX + " - screenY: " + d3.event.screenY);
                    menu(g.x + d3.mouse(this)[0] + 35, g.y + d3.mouse(this)[1] + 35, g);
                });

            node.on("mouseenter", function (g) {
                //console.log("mouseenter called");
                if (!isTransitioning && $rootScope.currentopenfile.file.FileName == strFileName) {
                    restoreLinksAndNodes();
                    highlightConnected(g);
                    fadeUnconnected(g);

                    d3.select(this).select("rect")
                      .style("fill", function (d) {
                          d.color = INFLOW_COLOR;
                          // d.color = d.netFlow > 0 ? INFLOW_COLOR : OUTFLOW_COLOR;
                          return d.color;
                      })
                       .style("stroke", "white")
                      .style("stroke-WIDTH", "4")
                      .style("fill-opacity", OPACITY.LINK_DEFAULT);

                    tooltip
                      .style("left", g.x + MARGIN.LEFT + "px")
                      .style("top", g.y + g.height + MARGIN.TOP + 15 + "px")
                      .transition()
                        .duration(TRANSITION_DURATION)
                        .style("opacity", 1).select(".value")
                        .text(function () {
                            var additionalInstructions = g.children.length ? "\n(Double click to expand)" : "";
                            return g.name;
                        });
                }
            });

            node.on("mouseleave", function () {
                //console.log("mouseleave called");
                if (!isTransitioning && $rootScope.currentopenfile.file.FileName == strFileName) {
                    hideTooltip();
                    restoreLinksAndNodes();
                }
            });

            node.filter(function (d) { return d.children.length; })
              .on("dblclick", showHideChildren);

            // allow nodes to be dragged to new positions
            node.call(d3.behavior.drag()
              .origin(function (d) { return d; })
              .on("dragstart", function () {
                  d3.select("#" + strFileName + " .context-menu").remove();
                  this.parentNode.appendChild(this);
              })
              .on("drag", dragmove));

            // add in the text for the nodes
            node.filter(function (d) {
                //return d.value !== 0;
                return true;
            })
              .select("text.node-name")
                .attr("x", -6)
               .attr("y", function (d) { return d.height / 2; })
                .attr("dy", ".35em")
                .attr("text-anchor", "end")
                .attr("transform", null)
                 .text(function (d) { return d.name; })
                .filter(function (d) { return d.x < WIDTH / 2; })
                .attr("x", function (d) {

                    var rectMaxWidth = CalculateRectangleWidth(d);
                    if (rectMaxWidth > 160) {
                        rectMaxWidth = rectMaxWidth + 20;
                        d.width = rectMaxWidth - 5;
                    }
                    else {
                        rectMaxWidth = rectMaxWidth + 6;
                    }
                    return rectMaxWidth;
                    //if (this.parentNode && d3.select(this.parentNode).select("rect")[0][0].getBBox().width>0) {
                    //    d.width = d3.select(this.parentNode).select("rect")[0][0].getBBox().width;
                    //    return d3.select(this.parentNode).select("rect")[0][0].getBBox().width + 6;
                    //}
                    //else {
                    //    return 6 + biHiSankey.nodeWidth();
                    //}
                })
                .attr("text-anchor", "start");

            link = svg.select("#links").selectAll("path.link")
              .data(biHiSankey.visibleLinks(), function (d) {
                  return d.id;
              });

            link.transition()
              .duration(TRANSITION_DURATION)
              .attr("d", path)
              .style("opacity", OPACITY.LINK_DEFAULT);

            link.exit().remove();

            linkEnter = link.enter().append("path")
              .attr("class", "link")
              .style("fill", "none");

            linkEnter.on('mouseenter', function (d) {
                if (!isTransitioning) {
                    showTooltip().select(".value").text(function () {
                        //if (d.direction > 0) {
                        return d.source.name + " → " + d.target.name + "\n" + d.type;
                        //}
                        //return d.target.name + " ← " + d.source.name + "\n" + d.type;
                    });

                    d3.select(this)
                       .style("stroke", function (d, i) {
                           return d.type == "onetoone" ? INFLOW_COLOR : OUTFLOW_COLOR;
                       })
                      .transition()
                        .duration(TRANSITION_DURATION / 2)
                        .style("opacity", OPACITY.LINK_HIGHLIGHT);
                }
            });

            linkEnter.on('mouseleave', function () {
                if (!isTransitioning) {
                    hideTooltip();

                    d3.select(this)
                       .style("stroke", function (d, i) {
                           return d.type == "onetoone" ? INFLOW_COLOR : OUTFLOW_COLOR;
                       })
                      .transition()
                        .duration(TRANSITION_DURATION / 2)
                        .style("opacity", OPACITY.LINK_DEFAULT);
                }
            });

            linkEnter.sort(function (a, b) { return b.thickness - a.thickness; })
              .classed("leftToRight", function (d) {
                  return d.direction > 0;
              })
              .classed("rightToLeft", function (d) {
                  return d.direction < 0;
              })
              .style("marker-end", function (d) {
                  //return 'url(#arrowHead)';
                  return d.type == "onetoone" ? 'url(#' + strFileName + 'arrowHeadOneToOne)' : 'url(#' + strFileName + 'arrowHeadOneToMany)';
              })
              .style("stroke", function (d, i) {
                  return d.type == "onetoone" ? INFLOW_COLOR : OUTFLOW_COLOR;
              })
              .style("opacity", 0)
              .transition()
                .delay(TRANSITION_DURATION)
                .duration(TRANSITION_DURATION)
                .attr("d", path)
                .style("stroke-WIDTH", function (d) { return Math.max(1, d.thickness); })
                .style("opacity", OPACITY.LINK_DEFAULT);

            collapser = svg.select("#collapsers").selectAll(".collapser")
              .data(biHiSankey.expandedNodes(), function (d) { return d.id; });

            collapserEnter = collapser.enter().append("g").attr("class", "collapser");

            collapserEnter.append("circle")
              .attr("r", COLLAPSER.RADIUS)
              .style("fill", function (d) {
                  d.color = colorScale(d.type.replace(/ .*/, ""));
                  return d.color;
              });

            collapserEnter
              .style("opacity", OPACITY.NODE_DEFAULT)
              .attr("transform", function (d) {
                  return "translate(" + (d.x + d.width / 2) + "," + (d.y + COLLAPSER.RADIUS) + ")";
              });

            collapserEnter.on("dblclick", showHideChildren);

            collapser.select("circle")
              .attr("r", COLLAPSER.RADIUS);

            collapser.transition()
              .delay(TRANSITION_DURATION)
              .duration(TRANSITION_DURATION)
              .attr("transform", function (d, i) {
                  return "translate("
                    + (COLLAPSER.RADIUS + i * 2 * (COLLAPSER.RADIUS + COLLAPSER.SPACING))
                    + ","
                    + (-COLLAPSER.RADIUS - OUTER_MARGIN)
                    + ")";
              });

            collapser.on("mouseenter", function (g) {
                if (!isTransitioning) {
                    showTooltip().select(".value")
                      .text(function () {
                          return g.name + "\n(Double click to collapse)";
                      });

                    var highlightColor = highlightColorScale(g.type.replace(/ .*/, ""));

                    d3.select(this)
                      .style("opacity", OPACITY.NODE_HIGHLIGHT)
                      .select("circle")
                        .style("fill", highlightColor);

                    node.filter(function (d) {
                        return d.ancestors.indexOf(g) >= 0;
                    }).style("opacity", OPACITY.NODE_HIGHLIGHT)
                      .select("rect")
                        .style("fill", highlightColor);
                }
            });

            collapser.on("mouseleave", function (g) {
                if (!isTransitioning) {
                    hideTooltip();
                    d3.select(this)
                      .style("opacity", OPACITY.NODE_DEFAULT)
                      .select("circle")
                        .style("fill", function (d) { return d.color; });

                    node.filter(function (d) {
                        return d.ancestors.indexOf(g) >= 0;
                    }).style("opacity", OPACITY.NODE_DEFAULT)
                      .select("rect")
                        .style("fill", function (d) { return d.color; });
                }
            });

            collapser.exit().remove();


        }

        //var exampleNodes = [{ "type": "Entity", "id": "entFile", "name": "entFile", "attributes": ["lstRecordLayout"] }, { "type": "Entity", "id": "entutlRecordLayout", "name": "entutlRecordLayout", "attributes": [] }, { "type": "Entity", "id": "entFileHdr", "name": "entFileHdr", "attributes": ["objFile", "lstStatusSummary", "lstFileHdrError", "objtest"] }, { "type": "Entity", "id": "entStatusSummary", "name": "entStatusSummary", "attributes": ["objFileHdr"] }, { "type": "Entity", "id": "entFileHdrError", "name": "entFileHdrError", "attributes": [] }];

        //var exampleLinks = [{ "source": "entFile", "target": "entutlRecordLayout", "value": 5, "type": "onetomany" }, { "source": "entFileHdr", "target": "entFile", "value": 5, "type": "onetoone" }, { "source": "entFileHdr", "target": "entutlRecordLayout", "value": 5, "type": "onetoone" }, { "source": "entFileHdr", "target": "entStatusSummary", "value": 5, "type": "onetomany" }, { "source": "entFileHdr", "target": "entFileHdrError", "value": 5, "type": "onetomany" }];



        function CalculateRectangleWidth(d) {
            var rectMaxWidth = biHiSankey.nodeWidth();
            for (var i = 0; i < d.attributes.length; i++) {
                if ((d.attributes[i].length * 6) > rectMaxWidth) {
                    rectMaxWidth = (d.attributes[i].length * 6);
                }
            }
            return rectMaxWidth;
        }

        biHiSankey
          .nodes(exampleNodes)
          .links(exampleLinks)
          .initializeNodes(function (node) {
              node.state = node.parent ? "contained" : "collapsed";
          })
          .layout(LAYOUT_INTERATIONS);

        disableUserInterractions(2 * TRANSITION_DURATION);

        update();
    }
    this.setData = function (aobjExampleNodes, aobjExampleLinks) {
        exampleNodes = aobjExampleNodes;
        exampleLinks = aobjExampleLinks;
    }
}]);