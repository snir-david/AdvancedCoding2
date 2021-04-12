#include "pch.h"
#include "CircleAnomalyDetector.h"
#include "minCircle.h"
using namespace std;



CircleAnomalyDetector::CircleAnomalyDetector()
{
	this->minCirc = Circle();
	this->tsCSV = TimeSeries();
	this->threshold = 0.5;
}

CircleAnomalyDetector::~CircleAnomalyDetector()
{
}

void CircleAnomalyDetector::learnNormal(const TimeSeries& ts)
{
	vector<string> atts = ts.gettAttributes();
	size_t len = ts.getRowSize();
	float** vals = (float**)malloc(atts.size() * sizeof(float*));
	for (int i = 0; i < atts.size(); i++) {
		vals[i] = (float*)malloc(len * sizeof(float));
	}
	if (vals == NULL) {
		exit(-1);
	}
	for (size_t i = 0; i < atts.size(); i++) {
		vector<float> x = ts.getAttributeData(atts[i]);
		for (size_t j = 0; j < len; j++) {
			vals[i][j] = x[j];
			//vals[i][j] = x[j];
		}
	}

	for (size_t i = 0; i < atts.size(); i++) {
		string f1 = atts[i];
		float max = 0;
		size_t jmax = 0;
		for (size_t j = i + 1; j < atts.size(); j++) {
			float p = abs(pearson(vals[i], vals[j], len));
			if (p > max) {
				max = p;
				jmax = j;
			}
		}
		string f2 = atts[jmax];
		Point** ps = toPoints(ts.getAttributeData(f1), ts.getAttributeData(f2));
		learnHelper(len, max, f1, f2, ps);

		// delete points
		for (size_t k = 0; k < len; k++)
			delete ps[k];
		delete[] ps;
	}
}

void CircleAnomalyDetector::learnHelper(size_t len, float p/*pearson*/, string f1, string f2, Point** ps) {
	if (p > threshold) {
		correlatedFeatures c;
		c.feature1 = f1;
		c.feature2 = f2;
		c.corrlation = p;
		Circle circle = findMinCircle(ps, len);
		c.circleCenter = circle.center;
		c.threshold = circle.radius * 1.1; // 10% increase
		cf.push_back(c);
	}
}

vector<AnomalyReport> CircleAnomalyDetector::detect(const TimeSeries& ts)
{
	vector<AnomalyReport> v;
	tsCSV = ts;
	for_each(cf.begin(), cf.end(), [&v, &ts, this](correlatedFeatures c) {
		vector<float> x = ts.getAttributeData(c.feature1);
		vector<float> y = ts.getAttributeData(c.feature2);
		for (size_t i = 0; i < x.size(); i++) {
			Point p(x[i], y[i]);
			if (isAnomalous(p, c.circleCenter, c)) {
				string d = c.feature1 + " - " + c.feature2;
				v.push_back(AnomalyReport(d, (i + 1)));
			}
		}
		});
	return v;
}
	
bool CircleAnomalyDetector::isAnomalous(Point x, Point y, correlatedFeatures c) {
	return (dist(x,y) < c.corrlation);
}

Point** CircleAnomalyDetector::toPoints(vector<float> x, vector<float> y) {
	Point** ps = new Point * [x.size()];
	for (size_t i = 0; i < x.size(); i++) {
		ps[i] = new Point(x[i], y[i]);
	}
	return ps;
}