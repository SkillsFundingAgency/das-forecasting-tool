function ShowFundingEstimate() {
    this.apprenticeshipRoleSelectId = 'choose-apprenticeship'
    this.apprenticeshipRole = document.getElementById(this.apprenticeshipRoleSelectId)
    this.apprenticeshipNumber = document.getElementById('no-of-app')
    this.numberOfMonthsField = document.getElementById('apprenticeship-length')
    this.apprenticeshipDateMonth = document.getElementById('startDateMonth')
    this.apprenticeshipDateYear = document.getElementById('startDateYear')
    this.editMode = false
    this.selectedStandardId = 0
    this.numberOfApprentices = 0
    this.numberOfMonths = 0
    this.startMonth = null
    this.startYear = null
    this.startDate = null
    this.monthsRemaining = 0
    this.estimate = 0
    this.showEstimate = false
}

ShowFundingEstimate.prototype.init = function() {
    this.autoComplete()
    this.setupEvents()
    this.pageLoad()
    this.checkFieldValues()
}

ShowFundingEstimate.prototype.pageLoad = function () {
    if (this.apprenticeshipRole.value !== "") {
      this.selectedStandardId = this.apprenticeshipRole.value
    }
    if (this.apprenticeshipNumber.value !== "") {
      this.numberOfApprentices = this.apprenticeshipNumber.value
    }
    if (this.numberOfMonthsField.value !== "") {
      this.numberOfMonths = this.numberOfMonthsField.value
    }
    if (this.apprenticeshipDateMonth.value !== "") {
      this.startMonth = this.apprenticeshipDateMonth.value
      this.dateChange()
    }
    if (this.apprenticeshipDateYear.value !== "") {
      this.startYear = this.apprenticeshipDateYear.value
      this.dateChange()
    }
  }


ShowFundingEstimate.prototype.setupEvents = function () {
    var that = this
    this.apprenticeshipNumber.onkeyup = function () {
        that.numberOfApprentices = this.value | 0
        that.checkFieldValues()
    }
    this.numberOfMonthsField.onkeyup = function () {
      that.numberOfMonths = this.value | 0
      that.checkFieldValues()
  }
    this.apprenticeshipDateMonth.onkeyup = function () {
        that.startMonth = this.value
        that.dateChange()
        that.checkFieldValues()
    }
    this.apprenticeshipDateYear.onkeyup = function () {
        that.startYear = this.value
        that.dateChange()
        that.checkFieldValues()
    }
}

ShowFundingEstimate.prototype.dateChange = function () {
    this.startDate = new Date(this.startYear, this.startMonth - 1, 1)
    this.monthsRemaining = this.calculateMonthsDifference(this.startDate);
}


ShowFundingEstimate.prototype.autoComplete = function() {
    var that = this
    accessibleAutocomplete.enhanceSelectElement({
        selectElement: that.apprenticeshipRole,
        minLength: 2,
        autoselect: false,
        defaultValue: '',
        displayMenu: 'overlay',
        placeholder: '',
        showAllValues: true,
        onConfirm: function (opt) {
            var roleChange = false
            var txtInput = document.querySelector('#' + that.apprenticeshipRoleSelectId);
            var searchString = opt || txtInput.value;
            var requestedOption = [].filter.call(this.selectElement.options,
                function (option) {
                return (option.textContent || option.innerText) === searchString
                }
            )[0];
            if (requestedOption) {
                requestedOption.selected = true;
                if (requestedOption.value) {
                  if (that.selectedStandardId != requestedOption.value) {
                    roleChange = true
                  }
                  that.selectedStandardId = requestedOption.value
                } else {
                that.selectedStandardId = 0
                }
            } else {
                this.selectElement.selectedIndex = 0;
                that.selectedStandardId = 0
            }

            that.checkFieldValues()
            if (roleChange) {
              that.roleHasChanged()
            }
            console.log('selectedStandardId = ' + that.selectedStandardId)
        }
    });
}

ShowFundingEstimate.prototype.getCourseApiUrl = function () {
  var pathArray = window.location.pathname.split("/");
  var indexMax = pathArray.length - (this.editMode ? 2 : 1); 
  var newPathname = "";
  for (i = 1; i < indexMax; i++) {
      newPathname += "/";
      newPathname += pathArray[i];
  }
  return window.location.origin + newPathname + "/course";
}

ShowFundingEstimate.prototype.roleHasChanged = function () {
    var that = this
    var paramObj = {
      CourseId: this.selectedStandardId
    }
    var params = new URLSearchParams(paramObj).toString();
    var apiurl = this.getCourseApiUrl() + "?" + params
     
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function() {
        if (xhr.readyState === 4) {
          try {
              var response = JSON.parse(xhr.responseText)
              console.log(response.numberOfMonths)
              that.numberOfMonthsField.value = response.numberOfMonths
              that.numberOfMonths = response.numberOfMonths
          } catch (e) {
              console.log(e)
          }
        }
    }
    xhr.open("GET", apiurl, true);
    xhr.send(JSON.stringify(paramObj));
}

ShowFundingEstimate.prototype.getEstimate = function () {
    var that = this
    var paramObj = {
      CourseId: this.selectedStandardId,
      NumberOfApprentices: this.numberOfApprentices,
      NumberOfMonths: this.numberOfMonths,
      StartDate: this.startDate
    }
    var params = new URLSearchParams(paramObj).toString();
    var apiurl = this.getCourseApiUrl() + "?" + params
    var xhr = new XMLHttpRequest();
    xhr.onreadystatechange = function() {
        if (xhr.readyState === 4) {
          try {
              var response = JSON.parse(xhr.responseText)
              that.showEstimate = true
              that.updateUI()
          } catch (e) {
              that.showEstimate = false
              that.updateUI()
          }
        }
    }
    xhr.open("GET", apiurl, true);
    xhr.send();
}

ShowFundingEstimate.prototype.calculateMonthsDifference = function(futureDate) {
    var date = new Date();
    var firstOfCurrentMonth = new Date(date.getFullYear(), date.getMonth(), 1);
    var difference = (futureDate.getTime() - firstOfCurrentMonth.getTime()) / 1000;
    difference = difference / (60 * 60 * 24 * 7 * 4);
    return Math.round(difference) + 1;
  }
  
ShowFundingEstimate.prototype.checkFieldValues = function () {
    if (this.selectedStandardId.length > 0 && this.numberOfMonths > 0 && this.numberOfApprentices > 0 && this.monthsRemaining > 0) {
      this.showEstimate = true
      this.getEstimate()
    } else {
      this.showEstimate = false
      this.updateUI()
    }
}
  
ShowFundingEstimate.prototype.updateUI = function () {
    if (this.showEstimate) {
      console.log('Show estimate')
    } else {
      console.log('Hide estimate')
    }
}

var showFundingEstimate = new ShowFundingEstimate()
showFundingEstimate.init()