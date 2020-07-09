        window.onload = function () {

            var myLine = new RGraph.Line('cvs', [500, 1000, 1200, 1600, 1800, 1450, 1200, 1800, 1900, 1980, 2400, 2500], [1500, 1400, 1800, 1800, 2000, 2200, 1900, 1900, 1850, 2000, 2250, 2320]);
            myLine.Set('chart.labels', ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']);
            myLine.Set('chart.gutter.left', 40);
            myLine.Set('chart.gutter.right', 15);
            myLine.Set('chart.gutter.bottom', 20);
            myLine.Set('chart.colors', ['#87AFFB', '#8DCC6B']);
            //myLine.Set('chart.units.post', '%');
            myLine.Set('chart.linewidth', 5);
            myLine.Set('chart.hmargin', 15);
            myLine.Set('chart.text.color', '#333');
            myLine.Set('chart.text.font', 'Arial');
            myLine.Set('chart.background.grid.autofit', true);
            myLine.Set('chart.background.grid.autofit.numvlines', 11);
            myLine.Set('chart.shadow', false);
//            myLine.Set('chart.shadow.color', 'rgba(20,20,20,0.3)');
//            myLine.Set('chart.shadow.blur', 10);
//            myLine.Set('chart.shadow.offsetx', 0);
//            myLine.Set('chart.shadow.offsety', 0);
            myLine.Set('chart.background.grid.vlines', false);
            myLine.Set('chart.background.grid.border', true);
            myLine.Set('chart.noxaxis', true);
//            myLine.Set('chart.title', 'An example Line chart');
            myLine.Set('chart.axis.color', '#666');
            myLine.Set('chart.text.color', '#666');
            myLine.Set('chart.spline', true);

            /**
            * Use the Trace animation to show the chart
            */
            if (RGraph.isOld()) {
                // IE7/8 don't support shadow blur, so set custom shadow properties
                myLine.Set('chart.shadow.offsetx', 3);
                myLine.Set('chart.shadow.offsety', 3);
                myLine.Set('chart.shadow.color', '#aaa');
                myLine.Draw();
            } else {
                myLine.Set('chart.tooltips', [
                                              '<b>January</b><br />Started off the year quite averagely',
                                              '<b>February</b><br />Better than January, rising quite nicely',
                                              '<b>March</b><br />March was quite a spike',
                                              '<b>April</b><br />Rising very impressively',
                                              '<b>May</b><br />Dropping after the last two month spike',
                                              '<b>June</b><br />Still dropping',
                                              '<b>July</b><br />The fall is beginning to subside, but still dropping',
                                              '<b>August</b><br />A good rise after the fall',
                                              '<b>September</b><br />A very good peak',
                                              '<b>October</b><br />The peak is now subsiding',
                                              '<b>November</b><br />Still subsiding',
                                              '<b>December</b><br />Rising again after the last fall',


                                              '<b>January</b><br />Started off the year quite averagely as with Robert',
                                              '<b>February</b><br />Rising as with Robert',
                                              '<b>March</b><br />Not as good as Robert, but OK',
                                              '<b>April</b><br />Dropping after the peak',
                                              '<b>May</b><br />The low point as with last year',
                                              '<b>June</b><br />Rising higher than Robert',
                                              '<b>July</b><br />Consistent with last month',
                                              '<b>August</b><br />A nice high point',
                                              '<b>September</b><br />A low point',
                                              '<b>October</b><br />A nice rise',
                                              '<b>November</b><br />Falling for fall...',
                                              '<b>December</b><br />Rising again for Christmas'
                                             ]);


                RGraph.Effects.Line.jQuery.UnfoldFromCenterTrace(myLine, { 'duration': 1000 });
            }
        }
