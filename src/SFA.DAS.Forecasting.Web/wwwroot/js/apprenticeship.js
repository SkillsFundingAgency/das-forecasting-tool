function ShowFundingEstimate() {
    this.apprenticeshipRoleSelectId = 'choose-apprenticeship'
    this.apprenticeshipRole = document.getElementById(this.apprenticeshipRoleSelectId)
    this.apprenticeshipNumber = document.getElementById('no-of-app')
    this.numberOfMonthsField = document.getElementById('apprenticeship-length')
    this.apprenticeshipDateMonth = document.getElementById('startDateMonth')
    this.apprenticeshipDateYear = document.getElementById('startDateYear')
    this.totalCostField = document.getElementById('total-funding-cost');
    this.editMode = false
    this.selectedStandardId = 0
    this.numberOfApprentices = 0
    this.numberOfMonths = 0
    this.startMonth = null
    this.startYear = null
    this.startDate = null
    this.monthsRemaining = 0
    this.totalFundingCap = 0
    this.fundingCap = 0
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
              that.response = response
              that.numberOfMonthsField.value = response.numberOfMonths
              that.numberOfMonths = response.numberOfMonths
              that.updateUI()
          } catch (e) {
              console.log(e)
          }
        }
    }
    xhr.open("GET", apiurl, true);
    xhr.send(JSON.stringify(paramObj));
}

ShowFundingEstimate.prototype.calculateMonthsDifference = function(futureDate) {
    var date = new Date();
    var firstOfCurrentMonth = new Date(date.getFullYear(), date.getMonth(), 1);
    var difference = (futureDate.getTime() - firstOfCurrentMonth.getTime()) / 1000;
    difference = difference / (60 * 60 * 24 * 7 * 4);
    return Math.round(difference) + 1;
  }
  
ShowFundingEstimate.prototype.checkFieldValues = function () {
  this.showEstimate = false
  if (this.selectedStandardId.length > 0 && this.numberOfMonths > 0 && this.numberOfApprentices > 0 && this.monthsRemaining > 0) {
    this.showEstimate = true
  }
  this.updateUI()
}
  
ShowFundingEstimate.prototype.updateUI = function () {
    var infoPanel = document.getElementById("details-about-funding");
    var calcPanel = document.getElementById("details-about-funding-calculated");
    
    if (this.showEstimate) {
      this.refreshCalculation()
      infoPanel.style.display = "none";
      calcPanel.style.display = "block";
    } else {
      this.totalCostField.value = 0
      infoPanel.style.display = "block";
      calcPanel.style.display = "none";
    }
}

ShowFundingEstimate.prototype.refreshCalculation = function() {
  var calucation = AddEditApprentiecships.calculateFundingCap(this.startDate, this.response)
  var fundingCap = calucation ? calucation.fundingCap : 0;
  this.fundingCap = fundingCap;
  this.totalFundingCap = this.fundingCap * this.numberOfApprentices;
  this.updateView();  
}

ShowFundingEstimate.prototype.updateView = function() {
  var fc = AddEditApprentiecships.toGBP(this.fundingCap)
  var tfc = AddEditApprentiecships.toGBP(this.totalFundingCap || 0)
  document.getElementById("funding-cap-details").innerHTML = fc;
  document.getElementById("apprentice-count-details").innerHTML = this.numberOfApprentices;
  document.getElementById("total-cap-details").innerHTML = tfc;
  this.totalCostField.value = tfc.replace('£', '');
}

var showFundingEstimate = new ShowFundingEstimate()
showFundingEstimate.init()

var AddEditApprentiecships = {
  altFind: function (arr, callback) {
      for (var i = 0; i < arr.length; i++) {
          var match = callback(arr[i]);
          if (match) {
              return arr[i];
          }
      }
  },
  calculateFundingCap: function (date, model) {
      var today = new Date();
      var thisMonth = new Date(today.getFullYear(), today.getMonth(), 1, 0, 0, 0)

      if (date === undefined
          || date.toString() === "Invalid Date"
          || date < thisMonth
          || model === undefined
          || model.fundingBands == null ){
          return undefined;
      }

      var fundingBand = AddEditApprentiecships.altFind(model.fundingBands, 
          function (fb) {
            return date > AddEditApprentiecships.getDate(fb.FromDate) && (date < AddEditApprentiecships.getDate(fb.ToDate) || fb.ToDate == null);
          }) || model.fundingBands[model.fundingBands.length - 1];

      var result = {
        fundingCap: fundingBand.fundingCap
      };

      return result;
  },
  getDate: function (cSharpDate) {
      if (!cSharpDate)
          return cSharpDate;

      if (cSharpDate.indexOf('-') > -1) {
          return new Date(cSharpDate);
      }

      var stripedCsharpDate = cSharpDate.replace(/[^0-9 +]/g, '');
      return new Date(parseInt(stripedCsharpDate));
  },
  toGBP: function (data) {
      return data.toLocaleString('en-GB', { style: 'currency', currency: 'GBP' }).split('.')[0];
  },
  numberWithCommas: function (number) {
      var parts = number.toString().split('.');
      var partToProcess = parts[0];
      partToProcess = partToProcess.replace(/,/g, '');
      partToProcess = partToProcess.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
      parts[0] = partToProcess;
      return parts.join('.');
  }, 
  onlyAllowNumbers: function (event) {
      return event.metaKey ||
          event.which <= 0 ||
          event.which == 8 ||
          /[0-9]/.test(String.fromCharCode(event.which));
  }
};
