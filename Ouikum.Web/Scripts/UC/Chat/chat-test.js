
function Task(FromName, FromID, ToID, ReadCount) {
    //this.FromName = ko.observable(FromName);
    //this.FromID = ko.observable(FromID);
    //this.ToID = ko.observable(ToID);
    //this.ReadCount = ko.observable(ReadCount);

    this.FromName = FromName;
    this.FromID = FromID;
    this.ToID = ToID;
    this.ReadCount = ReadCount;
}

//TaskListViewModel.GetTask = ko.dependentObservable(function () {
//    this.tasks = ko.observableArray([
//        new Task("Steve", 1, 1, 10),
//        new Task("Bert", 2, 2, 5),
//        new Task("Tom", 2, 2, 5),
//    ]);
//    return this.Task;
//}, ticketDataViewModel);

function TaskListViewModel() {
    // Data    
    var tickets = $.connection.ticketHub;
    this.hub = $.connection.chatHub;
    this.tasks = ko.observableArray([]);

    var tasks = this.tasks;

    // Bahavior
    this.init = function () {
        console.log('init');
        this.hub.server.getUnread();
    };

    this.addTask = function () {
        this.tasks.push(new Task("test", 10, 10, 10));
    };

    this.totalUnRead = ko.computed(function () {
        var total = 0;
        for (var i = 0; i < this.tasks().length; i++) {
            total += this.tasks()[i].ReadCount;
        }
        return total;
    }, this);

    //this.totalUnRead = ko.computed(function () {
    //    var total = 0;
    //    for (var i = 0; i < this.tasks().length; i++) {
    //        total += this.tasks()[i].ReadCount;
    //    }

    //    return total;
    //}); 

    this.hub.client.showMessageUnRead = function (Toid, UnreadModel, TotalUnRead) {
        console.log('Toid : ' + Toid);
        console.log('total-unread : ' + TotalUnRead);
        console.log(UnreadModel);
        var collection = $.map(UnreadModel, function (item) {
            return new Task(item.FromName, item.FromID, item.ToID, item.ReadCount);
        });
        tasks(collection);
    };


};

$(function () {
    // Declare a proxy to reference the hub. 
    var taskModel = new TaskListViewModel();
    $.connection.hub.start(function () {
        taskModel.init();
    });
    ko.applyBindings(taskModel);
    // Create a function that the hub can call to broadcast messages.
    //chat.client.showMessageUnRead = function (Toid, UnreadModel, TotalUnRead) {
    //    // Add the message to the page. 

    //    console.log('Toid : ' + Toid);
    //    console.log('total-unread : ' + TotalUnRead);
    //    console.log(UnreadModel);
    //    if (TotalUnRead > 0) {
    //        console.log('Toid : ' + Toid);
    //        console.log('total-unread : ' + TotalUnRead);
    //        console.log(UnreadModel); 
    //    }
    //    //TaskListViewModel.testTask(5);

    //    //   $('.tag-' + Toid).text(SumUnRead);
    //};
    //$.connection.hub.start().done(function () {
    //    chat.server.getUnread();
    //    // Set initial focus to message input box.   
    //});
    // Start the connection.


});
